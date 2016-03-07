using System.Collections.Generic;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
{
    public interface IPlayerRespository : IRepositoryBase<Player>
    {
        Player FindOrCreate(string playerName);
    }
}