using System;
using System.Collections.Generic;

using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using SC2LiquipediaStatistics.Utilities.Domain;

namespace SC2Statistics.SC2Domain.Model
{
    public class Event : EntityBase
    {
        public virtual long Id { get; set; }

        public virtual bool IsActive { get; set; }

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
            IsActive = true;
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

            if (subEvent.Expansion == Expansion.Undefined)
                subEvent.Expansion = Expansion;

            if (subEvent.LiquipediaTier == LiquipediaTier.Undefined)
                subEvent.LiquipediaTier = LiquipediaTier;

            SubEvents.Add(subEvent);
        }

        public void Merge(Event eventToMergeFrom)
        {
            Name = eventToMergeFrom.Name;
            LiquipediaReference = eventToMergeFrom.LiquipediaReference;
            StartDate = eventToMergeFrom.StartDate;
            EndDate = eventToMergeFrom.EndDate;
            LiquipediaTier = eventToMergeFrom.LiquipediaTier;
            PrizePool = eventToMergeFrom.PrizePool;
            Expansion = eventToMergeFrom.Expansion;
            SubEvents.Clear();

            foreach (var subEvent in eventToMergeFrom.SubEvents)
            {
                AddSubEvent(subEvent);
            }
        }
    }
}
