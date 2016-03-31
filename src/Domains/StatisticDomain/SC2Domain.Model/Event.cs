using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using SC2LiquipediaStatistics.Utilities.Domain;

namespace SC2Statistics.SC2Domain.Model
{
    public class Event : EntityBase
    {
        public virtual long Id { get; set; }

        [NotNullValidator]
        public virtual string Name { get; set; }

        public virtual Tier Tier { get; set; }

        public virtual string PrizePool { get; set; }

        public virtual IList<Match> Matches { get; set; }

        public virtual int AligulacId { get; set; }

        public virtual string AligulacReference { get; set; }

        public Event()
        {
            Tier = Tier.Undefined;
            Matches = new List<Match>();
        }

        public int GetTotalMatches()
        {
            return Matches.Count;
        }

        public void AddMatch(Match match)
        {
            if (Matches.Contains(match))
                return;

            match.Event = this;
            Matches.Add(match);
        }
    }
}
