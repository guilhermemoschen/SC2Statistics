using System.Collections.Generic;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.SC2Domain.Repository
{
    public interface IMatchRepository : IRepositoryBase<Match>
    {
        IList<Match> FindMatchesByPlayerAndExpansion(long playerId, Expansion expansion);

        IList<Match> FindMatchesByPlayerAndEvent(long playerId, long eventId);

        Match GetLatestMatchFromPlayer(int aligulacPlayerId);
    }
}