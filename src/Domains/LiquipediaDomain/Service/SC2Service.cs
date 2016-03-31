using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

using AutoMapper;

using Microsoft.Practices.EnterpriseLibrary.Validation;

using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Web;

using SC2Statistics.Proxy.Aligulac;
using SC2Statistics.SC2Domain.Model;
using SC2Statistics.SC2Domain.Repository;

namespace SC2Statistics.SC2Domain.Service
{
    public class SC2Service : ISC2Service
    {
        public IAligulacService AligulacService { get; protected set; }

        public IEventRepository EventRepository { get; protected set; }

        public IPlayerRespository PlayerRepository { get; protected set; }

        public IMatchRepository MatchRepository { get; protected set; }

        public IMapper Mapper { get; protected set; }

        public ILogger Logger { get; protected set; }

        public SC2Service(IAligulacService aligulacService, IEventRepository eventRepository, IPlayerRespository playerRespository, IMatchRepository matchRepository, IMapper mapper, ILogger logger)
        {
            AligulacService = aligulacService;
            EventRepository = eventRepository;
            PlayerRepository = playerRespository;
            MatchRepository = matchRepository;
            Mapper = mapper;
            Logger = logger;
        }


        public Event CreateEvent(Event sc2Event)
        {
            if (sc2Event == null)
                throw new ArgumentNullException("sc2Event");

            if (!sc2Event.IsValid)
                throw new ValidationException(sc2Event.ValidationResults);

            var existentEvent = EventRepository.FindByReference(sc2Event.AligulacReference);
            if (existentEvent == null)
            {
                using (var scope = new TransactionScope())
                {
                    EventRepository.Save(sc2Event);
                    scope.Complete();
                }
            }

            return existentEvent ?? sc2Event;
        }

        public IEnumerable<Player> FindPlayers(string tag, int pageIndex = 0, int pageSize = 20)
        {
            return PlayerRepository.FindByTag(tag, pageIndex, pageSize);
        }

        public IEnumerable<Player> FindAllPlayers()
        {
            return PlayerRepository.FindAll();
        }

        public IEnumerable<Player> FindAllPlayers(int pageIndex, int pageSize)
        {
            return PlayerRepository.FindAllAndOrderBy(x => x.AligulacId, pageIndex, pageSize);
        }

        public void UpdateEvent(Event sc2Event)
        {
            if (sc2Event == null)
                throw new ArgumentNullException("sc2Event");

            using (var scope = new TransactionScope())
            {
                EventRepository.Merge(sc2Event);
                scope.Complete();
            }
        }

        public Event LoadEvent(long eventId)
        {
            return EventRepository.Load(eventId);
        }

        public IList<Event> FindEventsByPlayer(long playerId)
        {
            return EventRepository.FindEventsByPlayer(playerId);
        }

        public void DeleteEvent(long eventId)
        {
            using (var scope = new TransactionScope())
            {
                var existentEvent = EventRepository.Load(eventId);
                EventRepository.Delete(existentEvent);
                scope.Complete();
            }
        }

        public void UpdateAllPlayers()
        {
            var bigestPlayerAligulacId = PlayerRepository.GetBigestPlayerAligulacId();
            var players = AligulacService.FindAllPlayers(bigestPlayerAligulacId);

            using (var scope = new TransactionScope())
            {
                foreach (var player in players)
                {
                    if (!player.IsValid)
                        throw new ValidationException(player.ValidationResults);

                    PlayerRepository.Save(player);
                }
                scope.Complete();
            }
        }

        public void LoadLatestPlayerMatches(int aligulacPlayerId, Expansion expansion)
        {
            var player = PlayerRepository.FindByAligulacId(aligulacPlayerId);

            if (player == null)
                throw new ValidationException("Invalid Player.");

            var lastestMatch = MatchRepository.GetLatestMatchFromPlayer(player.AligulacId);
            var newMatches = AligulacService.FindMatches(player.AligulacId, expansion, (lastestMatch != null)?lastestMatch.Date:null);
            UpdatePlayersReference(player, newMatches);
            UpdateEventsReference(newMatches);

            foreach (var match in newMatches)
            {
                CreateMatch(match);
            }
        }

        private void CreateMatch(Match match)
        {
            if (match == null)
                throw new ArgumentNullException("match");

            if (!match.IsValid)
                throw new ValidationException("The Match is invalid");

            using (var scope = new TransactionScope())
            {
                MatchRepository.Save(match);
                scope.Complete();
            }
        }

        private void UpdateEventsReference(IEnumerable<Match> matches)
        {
            var cachedEvents = new Collection<Event>();

            foreach (var match in matches)
            {
                var cachedEvent = cachedEvents.FirstOrDefault(x => x.AligulacId == match.Event.AligulacId);
                if (cachedEvent != null)
                {
                    match.Event = cachedEvent;
                    continue;
                }

                var existentEvent = EventRepository.FindByAligulacId(match.Event.AligulacId);
                if (existentEvent != null)
                {
                    match.Event = existentEvent;
                    continue;
                }

                var newEvent = CreateEvent(match.Event);
                match.Event = newEvent;
                cachedEvents.Add(newEvent);
            }
        }

        private void UpdatePlayersReference(Player mainPlayer, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                if (match.Player1.AligulacId == mainPlayer.AligulacId)
                {
                    match.Player1 = mainPlayer;
                    match.Player2 = PlayerRepository.FindByAligulacId(match.Player2.AligulacId);
                }
                else if (match.Player2.AligulacId == mainPlayer.AligulacId)
                {
                    match.Player1 = PlayerRepository.FindByAligulacId(match.Player1.AligulacId);
                    match.Player2 = mainPlayer;
                }
                else
                {
                    throw new ValidationException("Invalid Match");
                }
            }
        }
    }
}
