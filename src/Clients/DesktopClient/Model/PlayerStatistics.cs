using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class PlayerStatistics
    {
        public Player Player { get; set; }

        public int TotalMatches { get; set; }
        public int TotalWins { get; set; }
        public decimal WinRate { get; set; }

        public decimal WinRateXTerran { get; set; }
        public int TotalWinsAgainstTerran { get; set; }
        public int TotalMatchesAgainstTerran { get; set; }

        public decimal WinRateXProtoss { get; set; }
        public int TotalWinsAgainstProtoss { get; set; }
        public int TotalMatchesAgainstProtoss { get; set; }

        public decimal WinRateXZerg { get; set; }
        public int TotalWinsAgainstZerg { get; set; }
        public int TotalMatchesAgainstZerg { get; set; }

        public decimal WinRateXKoreans { get; set; }
        public int TotalWinsAgainstKoreans { get; set; }
        public int TotalMatchesAgainstKoreans { get; set; }

        public decimal WinRateXForeigners { get; set; }
        public int TotalWinsAgainstForeigners { get; set; }
        public int TotalMatchesAgainstForeigners { get; set; }
    }
}
