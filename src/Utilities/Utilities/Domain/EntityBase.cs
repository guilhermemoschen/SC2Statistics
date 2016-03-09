using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace SC2LiquipediaStatistics.Utilities.Domain
{
    public abstract class EntityBase
    {
        public virtual ValidationResults ValidationResults { get; protected set; }

        public virtual bool IsValid
        {
            get
            {
                ValidationResults = Validate();
                return ValidationResults.IsValid;
            }
        }

        public ValidationResults Validate()
        {
            return Validation.Validate(this);
        }
    }
}
