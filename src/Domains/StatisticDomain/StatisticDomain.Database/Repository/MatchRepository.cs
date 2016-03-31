using System.Collections.Generic;
using System.Linq;

using NHibernate.Linq;

using SC2Statistics.StatisticDomain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.StatisticDomain.Database.Repository
{
    public class MatchRepository : RepositoryBase<Match>, IMatchRepository
    {
        public IList<Match> FindMatchesByPlayerAndExpansion(long playerId, Expansion expansion)
        {
            return Session.Query<Match>()
                .Where(x => 
                    (x.Player1.Id == playerId || x.Player2.Id == playerId) && 
                    (x.Expansion == expansion)
                )
                .ToList();
        }

        public IList<Match> FindMatchesByPlayerAndEvent(long playerId, long eventId)
        {
            return Session.Query<Match>()
                .Where(x =>
                    (x.Event.Id == eventId) &&
                    (x.Player1.Id == playerId || x.Player2.Id == playerId)
                )
                .ToList();
        }

        public Match GetLatestMatchFromPlayer(int aligulacPlayerId)
        {
            return Session.Query<Match>()
                .Where(x => x.Player1.AligulacId == aligulacPlayerId || x.Player2.AligulacId == aligulacPlayerId)
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();
        }
    }
}
