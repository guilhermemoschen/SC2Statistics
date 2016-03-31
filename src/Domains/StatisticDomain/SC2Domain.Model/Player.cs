using System;
using System.Collections.Generic;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using SC2LiquipediaStatistics.Utilities.Domain;

namespace SC2Statistics.SC2Domain.Model
{
    public class Player : EntityBase
    {
        public virtual long Id { get; set; }

        public virtual int AligulacId { get; set; }

        [NotNullValidator]
        public virtual string AligulacReferenceUrl { get; set; }

        [NotNullValidator]
        public virtual string Tag { get; set; }

        public virtual string Name { get; set; }

        public virtual string LiquipediaName { get; set; }

        public virtual Race Race { get; set; }

        public virtual DateTime? Birthday { get; set; }

        /// <summary>
        /// Country (ISO 3166-1 alpha-2)
        /// </summary>
        public virtual string Country { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }
    }
}
