using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Practices.EnterpriseLibrary.Validation;

using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Web;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.SC2Domain.Repository;

namespace SC2Statistics.SC2Domain.Service
{
    public class SC2Service : ISC2Service
    {
        public IParseService ParseService { get; protected set; }

        public IEventRepository EventRepository { get; protected set; }

        public IPlayerRespository PlayerRepository { get; protected set; }

        public ILogger Logger { get; protected set; }

        public SC2Service(IParseService parseService, IEventRepository eventRepository, IPlayerRespository playerRespository, ILogger logger)
        {
            ParseService = parseService;
            EventRepository = eventRepository;
            PlayerRepository = playerRespository;
            Logger = logger;
        }

        public IList<Event> FindMainEvents()
        {
            return EventRepository
                .FindMainEvents();
        }

        public void CreateEvent(Event sc2Event)
        {
            if (sc2Event == null)
                throw new ArgumentNullException("sc2Event");

            if (!sc2Event.IsValid)
                throw new ValidationException(sc2Event.ValidationResults);

            using (var scope = new TransactionScope())
            {
                EventRepository.Save(sc2Event);
                scope.Complete();
            }
        }

        public IList<Player> FindAllPlayers()
        {
            return PlayerRepository
                .FindAll()
                .OrderBy(x => x.Name)
                .ToList();
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
    }
}
