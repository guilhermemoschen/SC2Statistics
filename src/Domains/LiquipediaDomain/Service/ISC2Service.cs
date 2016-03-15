using System.Collections.Generic;
using System.Threading.Tasks;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Service
{
    public interface ISC2Service
    {
        IList<Event> FindMainEvents();

        Event CreateEvent(Event sc2Event);

        IList<Player> FindAllPlayers();

        void UpdateEvent(Event sc2Event, IEnumerable<long>  eventsIdToActive = null, IEnumerable<long> eventsIdToDeactive = null);

        Event LoadEvent(long eventId);

        IList<Event> FindEventsByPlayer(long playerId);

        void ActiveEvent(long eventId);

        void InactiveEvent(long eventId);

        void DeleteEvent(long eventId);

        void DeleteSubEvent(long eventId, long subEventId);
    }
}
