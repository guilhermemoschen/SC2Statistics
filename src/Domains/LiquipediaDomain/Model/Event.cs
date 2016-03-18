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

        public virtual bool IsActive { get; set; }

        [NotNullValidator]
        public virtual string Name { get; set; }

        [NotNullValidator]
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
            Expansion = Expansion.LegacyOfTheVoid;
        }

        public int GetTotalMatches()
        {
            var allSubEvents = GetSubAllEvents(this);
            return allSubEvents.Sum(x => x.Matches.Count) + Matches.Count;
        }

        public IList<Event> GetSubAllEvents(Event parentEvent)
        {
            var events = new List<Event>();
            foreach (var subEvent in parentEvent.SubEvents)
            {
                events.Add(subEvent);
                events.AddRange(GetSubAllEvents(subEvent));
            }

            return events;
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

        public void Merge(Event eventToMergeFrom, bool mergeMatches, bool mergeSubEvents)
        {
            Name = eventToMergeFrom.Name;
            LiquipediaReference = eventToMergeFrom.LiquipediaReference;
            StartDate = eventToMergeFrom.StartDate;
            EndDate = eventToMergeFrom.EndDate;
            LiquipediaTier = eventToMergeFrom.LiquipediaTier;
            PrizePool = eventToMergeFrom.PrizePool;
            Expansion = eventToMergeFrom.Expansion;

            if (mergeMatches)
            {
                Matches.Clear();
                foreach (var match in eventToMergeFrom.Matches)
                {
                    AddMatch(match);
                }
            }

            if (mergeSubEvents)
            {
                SubEvents.Clear();
                foreach (var subEvent in eventToMergeFrom.SubEvents)
                {
                    AddSubEvent(subEvent);
                }
            }
        }

        public void RemoveSubEvent(long subEventId)
        {
            var subEvent = SubEvents.FirstOrDefault(x => x.Id == subEventId);
            if (subEvent == null)
                throw new ValidationException("Invalid SubEvent Id");

            SubEvents.Remove(subEvent);
        }
    }
}
