using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class Match : ObservableObject
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public Event Event { get; set; }
        public IList<Game> Games { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Winner { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public Race Player1Race { get; set; }
        public Race Player2Race { get; set; }
        public MatchType Type { get; set; }
        public string LiquipediaReference { get; set; }

        public Match() { }
    }
}
