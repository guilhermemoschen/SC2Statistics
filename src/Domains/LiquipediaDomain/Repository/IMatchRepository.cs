using System.Collections.Generic;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
{
    public interface IMatchRepository
    {
        IList<Match> FindAll(Player player);
    }
}