using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2Statistics.StatisticDomain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.StatisticDomain.Database.Repository
{
    public interface IAligulacSynchronizationRepository : IRepositoryBase<AligulacSynchronization>
    {
        DateTime? GetLastUpdate(string entityName, int entityId);

        AligulacSynchronization GetAligulacSynchronization(string entityName, int entityId);
    }
}