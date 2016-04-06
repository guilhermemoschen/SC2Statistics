using System.Collections.Generic;

using GalaSoft.MvvmLight;

using SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class Player : ObservableObject
    {
        public int AligulacId { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public Race Race { get; set; }
        public string Country { get; set; }
    }
}
