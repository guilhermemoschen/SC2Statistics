using System;
using System.Collections.Generic;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using SC2LiquipediaStatistics.Utilities.Domain;

namespace SC2Statistics.SC2Domain.Model
{
    public class Event : EntityBase
    {
        public virtual long Id { get; set; }

        [NotNullValidator]
        public virtual string Name { get; set; }

        public virtual string LiquipediaReference { get; set; }

        public virtual DateTime? StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual LiquipediaTier LiquipediaTier { get; set; }

        public virtual string PrizePool { get; set; }

        public virtual Expansion Expansion { get; set; }

        public virtual IList<Event> SubEvents { get; set; }

        public virtual Event MainEvent { get; set; }

        public virtual IList<Match> Matches { get; set; }

        public Event()
        {
            Matches = new List<Match>();
            SubEvents = new List<Event>();
        }

        public void AddMatch(Match match)
        {
            if (Matches.Contains(match))
                return;

            match.Event = this;
            Matches.Add(match);
        }

        public void AddSubEvent(Event subEvent)
        {
            if (SubEvents.Contains(subEvent))
                return;

            subEvent.MainEvent = this;
            SubEvents.Add(subEvent);
        }
    }
}
