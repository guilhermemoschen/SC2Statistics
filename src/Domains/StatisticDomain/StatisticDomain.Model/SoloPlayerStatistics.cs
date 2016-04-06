using System.Collections.Generic;

namespace SC2Statistics.StatisticDomain.Model
{
    public class SoloPlayerStatistics
    {
        public Player Player { get; set; }

        public IEnumerable<Match> Matches { get; set; }
        public int Wins { get; set; }
        public decimal WinRate { get; set; }

        public decimal WinRateXTerran { get; set; }
        public int WinsXTerran { get; set; }
        public int MatchesXTerran { get; set; }

        public decimal WinRateXProtoss { get; set; }
        public int WinsXProtoss { get; set; }
        public int MatchesXProtoss { get; set; }

        public decimal WinRateXZerg { get; set; }
        public int WinsXZerg { get; set; }
        public int MatchesXZerg { get; set; }

        public decimal WinRateXKoreans { get; set; }
        public int WinsXKoreans { get; set; }
        public int MatchesXKoreans { get; set; }

        public decimal WinRateXForeigners { get; set; }
        public int WinsXForeigners { get; set; }
        public int MatchesXForeigners { get; set; }
    }
}
