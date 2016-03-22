using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class GroupMatches
    {
        public string Title { get; set; }
        public IEnumerable<Match> Matches { get; set; }
    }
}
