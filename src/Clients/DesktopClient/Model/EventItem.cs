using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class EventItem : ObservableObject
    {
        public bool Selected { get; set; }
        public string URL { get; set; }
    }
}
