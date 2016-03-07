using System.Collections.Generic;

using GalaSoft.MvvmLight;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class Player : ObservableObject
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
