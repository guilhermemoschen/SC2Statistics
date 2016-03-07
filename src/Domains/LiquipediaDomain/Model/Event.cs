using System;
using System.Collections.Generic;

namespace SC2Statistics.SC2Domain.Model
{
    public class Event
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string LiquipediaReference { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual LiquipediaTier LiquipediaTier { get; set; }
        public virtual string PrizePool { get; set; }
        public virtual Expansion Expansion { get; set; }

        public virtual IList<Match> Matches { get; set; }

        public Event()
        {
            Matches = new List<Match>();
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
