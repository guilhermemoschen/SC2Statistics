using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Linq;

using SC2Statistics.StatisticDomain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.StatisticDomain.Database.Repository
{
    public class AligulacSynchronizationRepository : RepositoryBase<AligulacSynchronization>, IAligulacSynchronizationRepository
    {
        public DateTime? GetLastUpdate(string entityName, int entityId)
        {
            var lastUpdate = Session.Query<AligulacSynchronization>()
                .FirstOrDefault(x => x.EntityName == entityName && x.EntityId == entityId);

            return lastUpdate != null ? lastUpdate.LastUpdate : null;
        }

        public AligulacSynchronization GetAligulacSynchronization(string entityName, int entityId)
        {
            return Session.Query<AligulacSynchronization>()
                .FirstOrDefault(x => x.EntityName == entityName && x.EntityId == entityId);
        }
    }
}
