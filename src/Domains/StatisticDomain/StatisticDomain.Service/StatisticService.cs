using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;

using Microsoft.SqlServer.Server;

using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Log;

using SC2Statistics.Proxy.Aligulac;
using SC2Statistics.StatisticDomain.Database.Repository;
using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Service
{
    public class StatisticService : IStatisticService
    {
        public IMatchRepository MatchRepository { get; protected set; }

        public IPlayerRespository PlayerRepository { get; protected set; }

        public IAligulacService AligulacService { get; protected set; }

        public IEventRepository EventRepository { get; protected set; }

        public IAligulacSynchronizationRepository AligulacSynchronizationRepository { get; protected set; }

        public ILogger Logger { get; protected set; }

        public StatisticService(IAligulacService aligulacService, IEventRepository eventRepository, IMatchRepository matchRepository, IPlayerRespository playerRespository, IAligulacSynchronizationRepository aligulacSynchronizationRepository, ILogger logger)
        {
            AligulacService = aligulacService;
            MatchRepository = matchRepository;
            EventRepository = eventRepository;
            PlayerRepository = playerRespository;
            AligulacSynchronizationRepository = aligulacSynchronizationRepository;
            Logger = logger;
        }

        public PlayerStatistics GeneratePlayerStatistics(long playerId, Expansion expansion)
        {
            var player = PlayerRepository.Load(playerId);

            if (player == null)
                throw new ArgumentNullException(nameof(playerId));

            var allMatches = MatchRepository.FindMatchesByPlayerAndExpansion(player.Id, expansion);

            if (!allMatches.Any())
                return null;

            var matchesWon = allMatches.Where(x => x.Winner != null && x.Winner.Id == player.Id).ToList();

            var statistics = new PlayerStatistics()
            {
                Player = player,
                WinRate = (decimal)matchesWon.Count / (decimal)allMatches.Count,
                TotalWins = matchesWon.Count,
                TotalMatches = allMatches.Count,
            };

            var matchesXZerg = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2Race == Race.Zerg) ||
                    (x.Player2.Id == player.Id && x.Player1Race == Race.Zerg)
                )
                .ToList();

            if (matchesXZerg.Any())
            {
                var matchesWonXZerg = matchesXZerg.Where(x => x.Winner != null && x.Winner.Id == player.Id).ToList();
                statistics.WinRateXZerg = (decimal)matchesWonXZerg.Count / (decimal)matchesXZerg.Count;
                statistics.TotalMatchesAgainstZerg = matchesXZerg.Count;
                statistics.TotalWinsAgainstZerg = matchesWonXZerg.Count;
            }

            var matchesXProtoss = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2Race == Race.Protoss) ||
                    (x.Player2.Id == player.Id && x.Player1Race == Race.Protoss)
                )
                .ToList();

            if (matchesXProtoss.Any())
            {
                var matchesWonXProtoss = matchesXProtoss.Where(x => x.Winner != null && x.Winner.Id == player.Id).ToList();
                statistics.WinRateXProtoss = (decimal)matchesWonXProtoss.Count / (decimal)matchesXProtoss.Count;
                statistics.TotalMatchesAgainstProtoss = matchesXProtoss.Count;
                statistics.TotalWinsAgainstProtoss = matchesWonXProtoss.Count;
            }

            var matchesXTerran = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2Race == Race.Terran) ||
                    (x.Player2.Id == player.Id && x.Player1Race == Race.Terran)
                )
                .ToList();

            if (matchesXTerran.Any())
            {
                var matchesWonXTerran = matchesXTerran.Where(x => x.Winner != null && x.Winner.Id == player.Id).ToList();
                statistics.WinRateXTerran = (decimal)matchesWonXTerran.Count / (decimal)matchesXTerran.Count;
                statistics.TotalMatchesAgainstTerran = matchesXTerran.Count;
                statistics.TotalWinsAgainstTerran = matchesWonXTerran.Count;
            }

            var matchesXKoreans = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2.Country == "KR") ||
                    (x.Player2.Id == player.Id && x.Player1.Country == "KR")
                )
                .ToList();

            if (matchesXKoreans.Any())
            {
                var matchesWonXKoreans = matchesXKoreans.Where(x => x.Winner != null && x.Winner.Id == player.Id).ToList();
                statistics.WinRateXKoreans = (decimal)matchesWonXKoreans.Count / (decimal)matchesXKoreans.Count;
                statistics.TotalMatchesAgainstKoreans = matchesXKoreans.Count;
                statistics.TotalWinsAgainstKoreans = matchesWonXKoreans.Count;
            }

            var matchesXForeigners = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2.Country != "KR") ||
                    (x.Player2.Id == player.Id && x.Player1.Country != "KR")
                )
                .ToList();

            if (matchesXForeigners.Any())
            {
                var matchesWonXForeigners = matchesXForeigners.Where(x => x.Winner != null && x.Winner.Id == player.Id).ToList();
                statistics.WinRateXForeigners = (decimal)matchesWonXForeigners.Count / (decimal)matchesXForeigners.Count;
                statistics.TotalMatchesAgainstForeigners = matchesXForeigners.Count;
                statistics.TotalWinsAgainstForeigners = matchesWonXForeigners.Count;
            }

            return statistics;
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

            if (!players.Any())
                return;

            Logger.Info($"Saving {players.Count()} player(s)");
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
            Logger.Info("Finished");
        }

        public void LoadLatestPlayerMatches(int aligulacPlayerId, Expansion expansion)
        {
            var player = PlayerRepository.FindByAligulacId(aligulacPlayerId);

            if (player == null)
                throw new ValidationException("Invalid Player.");

            var lastUpdate = AligulacSynchronizationRepository.GetLastUpdate(AligulacSynchronization.MatchEntityName, player.AligulacId);
            var newMatches = AligulacService.FindMatches(player.AligulacId, expansion, lastUpdate);

            if (!newMatches.Any())
                return;

            UpdatePlayersReference(player, newMatches);
            UpdateEventsReference(newMatches);

            foreach (var match in newMatches)
            {
                CreateMatch(match);
            }

            lastUpdate = newMatches.Max(x => x.Date);
            if (lastUpdate == null)
                return;

            CreateOrUpdateAligulacSynchronization(AligulacSynchronization.MatchEntityName, player.AligulacId, lastUpdate.Value);
        }

        private void CreateOrUpdateAligulacSynchronization(string entityName, int entityId, DateTime lastUpdate)
        {
            var existentSync = AligulacSynchronizationRepository.GetAligulacSynchronization(entityName, entityId);
            AligulacSynchronization sync;

            if (existentSync == null)
            {
                sync = new AligulacSynchronization()
                {
                    EntityName = entityName,
                    EntityId = entityId,
                    LastUpdate = lastUpdate,
                };
            }
            else
            {
                sync = existentSync;
                sync.LastUpdate = lastUpdate;
            }

            if (!sync.IsValid)
                throw new ValidationException(sync.ValidationResults);

            using (var scope = new TransactionScope())
            {
                if (sync.Id == 0)
                    AligulacSynchronizationRepository.Save(sync);
                else
                    AligulacSynchronizationRepository.Merge(sync);
                scope.Complete();
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
