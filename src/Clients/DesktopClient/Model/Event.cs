using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class Event : ValidatableObject
    {
        public Event()
        {
            ValidateObject();
        }

        public long Id { get; set; }

        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if (isActive == value)
                    return;

                ValidateAndSet(() => IsActive, ref isActive, value);
            }
        }

        private string name;
        [Required(ErrorMessage = "The event name is mandatory.")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value)
                    return;

                ValidateAndSet(() => Name, ref name, value);
            }
        }

        private string liquipediaReference;
        [Required(ErrorMessage = "The Liquipedia Reference is mandatory.")]
        public string LiquipediaReference
        {
            get
            {
                return liquipediaReference;
            }
            set
            {
                if (liquipediaReference == value)
                    return;

                ValidateAndSet(() => LiquipediaReference, ref liquipediaReference, value);
            }
        }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                if (startDate == value)
                    return;

                ValidateAndSet(() => StartDate, ref startDate, value);
            }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                if (endDate == value)
                    return;

                ValidateAndSet(() => EndDate, ref endDate, value);
            }
        }

        private Tier liquipediaTier;
        public Tier LiquipediaTier
        {
            get
            {
                return liquipediaTier;
            }
            set
            {
                if (liquipediaTier == value)
                    return;

                ValidateAndSet(() => LiquipediaTier, ref liquipediaTier, value);
            }
        }

        private string prizePool;
        public string PrizePool
        {
            get
            {
                return prizePool;
            }
            set
            {
                if (prizePool == value)
                    return;

                ValidateAndSet(() => PrizePool, ref prizePool, value);
            }
        }

        private Expansion expansion;
        public Expansion Expansion
        {
            get
            {
                return expansion;
            }
            set
            {
                if (expansion == value)
                    return;

                ValidateAndSet(() => Expansion, ref expansion, value);
            }
        }

        public IList<SubEvent> SubEvents { get; set; }

        public Event MainEvent { get; set; }

        public bool HasSubEvents
        {
            get
            {
                return SubEvents.Any();
            }
        }

        public int TotalMatches { get; set; }
    }
}