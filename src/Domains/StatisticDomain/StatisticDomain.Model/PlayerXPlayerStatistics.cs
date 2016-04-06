using System.Collections.Generic;

namespace SC2Statistics.StatisticDomain.Model
{
    public class PlayerXPlayerStatistics
    {
        public SoloPlayerStatistics Player1Statistics { get; set; }
        public SoloPlayerStatistics Player2Statistics { get; set; }

        public IEnumerable<Match> MatchesBetweenPlayers { get; set; }
        public int Player1WinsXPlayer2 { get; set; }
        public decimal Player1WinRateXPlayer2 { get; set; }
        public int Player2WinsXPlayer1 { get; set; }
        public decimal Player2WinRateXPlayer1 { get; set; }
    }
}
