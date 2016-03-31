using System;
using System.Collections.Generic;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.SC2Domain.Repository
{
    public interface IPlayerRespository : IRepositoryBase<Player>
    {
        Player FindOrCreate(string playerName);

        int GetBigestPlayerAligulacId();

        IEnumerable<Player> FindByTag(string tag, int pageIndex = 0, int pageSize = 20);

        Player FindByAligulacId(int aligulacId);
    }
}