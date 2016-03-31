using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using SC2Statistics.SC2Domain.Model;

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
        public BracketRound BracketRound { get; set; }
        public MatchFormat Format { get; set; }
        public string GroupName { get; set; }

        public bool IsPlayer1Winner
        {
            get { return Player1.AligulacId == Winner.AligulacId; }
        }

        public bool IsPlayer2Winner
        {
            get { return Player2.AligulacId == Winner.AligulacId; }
        }

        public Match() { }
    }
}
