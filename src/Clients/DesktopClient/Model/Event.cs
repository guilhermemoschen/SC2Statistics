using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class Event : ObservableObject
    {
        public long Id { get; set; }

        private string name;
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

                Set(() => Name, ref name, value);
            }
        }

        private string liquipediaReference;
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

                Set(() => LiquipediaReference, ref liquipediaReference, value);
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

                Set(() => StartDate, ref startDate, value);
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

                Set(() => EndDate, ref endDate, value);
            }
        }

        private LiquipediaTier liquipediaTier;
        public LiquipediaTier LiquipediaTier
        {
            get
            {
                return liquipediaTier;
            }
            set
            {
                if (liquipediaTier == value)
                    return;

                Set(() => LiquipediaTier, ref liquipediaTier, value);
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

                Set(() => PrizePool, ref prizePool, value);
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

                Set(() => Expansion, ref expansion, value);
            }
        }
    }
}
