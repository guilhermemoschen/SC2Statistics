using System.Collections.Generic;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
{
    public interface IMatchRepository
    {
        IList<Match> FindMatchesByPlayerAndExpansion(Player player, Expansion expansion);

        IList<Match> FindMatchesByPlayerAndEvent(long playerId, long eventId);
    }
}