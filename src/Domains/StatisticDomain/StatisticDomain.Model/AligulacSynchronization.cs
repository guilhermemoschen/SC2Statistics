using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using SC2LiquipediaStatistics.Utilities.Domain;

namespace SC2Statistics.StatisticDomain.Model
{
    public class AligulacSynchronization : EntityBase
    {
        public const string PlayerEntityName = "Player";
        public const string MatchEntityName = "Match";
        public const string EventEntityName = "Event";

        public virtual long Id { get; set; }

        [NotNullValidator]
        public virtual string EntityName { get; set; }

        public virtual int EntityId { get; set; }

        [NotNullValidator]
        public virtual DateTime? LastUpdate { get; set; }
    }
}
