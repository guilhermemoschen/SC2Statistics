using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Database.Mapping
{
    public class AligulacSynchronizationMap : ClassMapping<AligulacSynchronization>
    {
        public AligulacSynchronizationMap()
        {
            Table("AligulacSynchronizations");
            Lazy(false);

            Id(x => x.Id, mapper => mapper.Generator(Generators.Identity));
            Property(x => x.EntityName, mapper => mapper.NotNullable(true));
            Property(x => x.EntityId, mapper => mapper.NotNullable(true));
            Property(x => x.LastUpdate, mapper => mapper.NotNullable(true));
        }
    }
}
