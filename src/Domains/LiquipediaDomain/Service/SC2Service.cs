using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Practices.EnterpriseLibrary.Validation;

using NHibernate.Linq;

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

        public Event CreateEvent(Event sc2Event)
        {
            if (sc2Event == null)
                throw new ArgumentNullException("sc2Event");

            if (!sc2Event.IsValid)
                throw new ValidationException(sc2Event.ValidationResults);

            var existentEvent = EventRepository.FindByReference(sc2Event.LiquipediaReference);
            if (existentEvent == null)
            {
                using (var scope = new TransactionScope())
                {
                    EventRepository.Save(sc2Event);
                    scope.Complete();
                }
            }
            else
            {
                existentEvent.Merge(sc2Event, true, true);
                UpdateEvent(existentEvent);
            }

            return existentEvent ?? sc2Event;
        }

        public IList<Player> FindAllPlayers()
        {
            return PlayerRepository
                .FindAll()
                .OrderBy(x => x.Name)
                .ToList();
        }

        public void UpdateEvent(Event sc2Event, IEnumerable<long> eventsIdToActive = null, IEnumerable<long> eventsIdToDeactive = null)
        {
            if (sc2Event == null)
                throw new ArgumentNullException("sc2Event");

            using (var scope = new TransactionScope())
            {
                EventRepository.Merge(sc2Event);
                scope.Complete();

                if (eventsIdToActive != null)
                {
                    foreach (var subEventId in eventsIdToActive)
                    {
                        var subEvent = EventRepository.Load(subEventId);
                        subEvent.IsActive = true;
                        EventRepository.Merge(subEvent);
                    }
                }

                if (eventsIdToDeactive != null)
                {
                    foreach (var subEventId in eventsIdToDeactive)
                    {
                        var subEvent = EventRepository.Load(subEventId);
                        subEvent.IsActive = false;
                        EventRepository.Merge(subEvent);
                    }
                }
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

        public void ActiveEvent(long eventId)
        {
            using (new TransactionScope())
            {
                var existentEvent = EventRepository.Load(eventId);
                existentEvent.IsActive = true;
                EventRepository.Merge(existentEvent);
            }
        }

        public void InactiveEvent(long eventId)
        {
            using (new TransactionScope())
            {
                var existentEvent = EventRepository.Load(eventId);
                existentEvent.IsActive = false;
                EventRepository.Merge(existentEvent);
            }
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

        public void DeleteSubEvent(long eventId, long subEventId)
        {
            using (var scope = new TransactionScope())
            {
                var existentEvent = EventRepository.Load(eventId);
                existentEvent.RemoveSubEvent(subEventId);
                EventRepository.Merge(existentEvent);
                scope.Complete();
            }
        }
    }
}
