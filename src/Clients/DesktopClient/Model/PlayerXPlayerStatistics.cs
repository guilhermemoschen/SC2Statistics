using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class PlayerXPlayerStatistics
    {
        public SoloPlayerStatistics Player1Statistics { get; set; }
        public SoloPlayerStatistics Player2Statistics { get; set; }

        public int MatchesBetweenPlayers { get; set; }
        public int Player1WinsXPlayer2 { get; set; }
        public decimal Player1WinRateXPlayer2 { get; set; }
        public int Player2WinsXPlayer1 { get; set; }
        public decimal Player2WinRateXPlayer1 { get; set; }
    }
}
