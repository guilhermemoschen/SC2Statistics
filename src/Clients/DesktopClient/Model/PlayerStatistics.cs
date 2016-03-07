﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class PlayerStatistics
    {
        public Player Player { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public int TotalMatchesPlayedAgainstTerran { get; set; }
        public int TotalMatchesPlayedAgainstZerg { get; set; }
        public int TotalMatchesPlayedAgainstProtoss { get; set; }
        public decimal WinRate { get; set; }
        public decimal WinRateXZerg { get; set; }
        public decimal WinRateXProtoss { get; set; }
        public decimal WinRateXTerran { get; set; }
    }
}