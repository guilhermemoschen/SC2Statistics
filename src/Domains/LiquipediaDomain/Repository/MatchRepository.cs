using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Linq;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.SC2Domain.Repository
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
