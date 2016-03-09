using System.Collections.Generic;
using System.Threading.Tasks;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Service
{
    public interface ISC2Service
    {
        IList<Event> FindMainEvents();

        void CreateEvent(Event sc2Event);

        IList<Player> FindAllPlayers();

        void UpdateEvent(Event sc2Event);

        Event LoadEvent(long eventId);

        IList<Event> FindEventsByPlayer(long playerId);
    }
}