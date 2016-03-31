using System.Collections.Generic;

using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Service
{
    public interface ISC2Service
    {
        Event CreateEvent(Event sc2Event);

        IEnumerable<Player> FindPlayers(string tag, int pageIndex = 0, int pageSize = 20);

        IEnumerable<Player> FindAllPlayers(int pageIndex, int pageSize);

        IEnumerable<Player> FindAllPlayers();

        Event LoadEvent(long eventId);

        IList<Event> FindEventsByPlayer(long playerId);

        void UpdateAllPlayers();

        void LoadLatestPlayerMatches(int aligulacPlayerId, Expansion expansion);
    }
}
