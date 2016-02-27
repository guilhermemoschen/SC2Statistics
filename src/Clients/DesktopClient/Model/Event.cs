using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class Event : ObservableObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string LiquipediaReference { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public LiquipediaTier LiquipediaTier { get; set; }
        public string PrizePool { get; set; }
        public Expansion Expansion { get; set; }

        public IList<Match> Matches { get; set; }

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
