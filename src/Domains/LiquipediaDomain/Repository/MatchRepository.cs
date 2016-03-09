using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Linq;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
{
    public class MatchRepository : RepositoryBase<Match>, IMatchRepository
    {
        public IList<Match> FindMatchesByPlayerAndExpansion(Player player, Expansion expansion)
        {
            return Session.Query<Match>()
                .Where(x => 
                    (x.Player1.Id == player.Id || x.Player2.Id == player.Id) && 
                    (x.Event.Expansion == expansion)
                )
                .ToList();
        }
    }
}
