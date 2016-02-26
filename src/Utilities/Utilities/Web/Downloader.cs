using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.Utilities.Web
{
    public class Downloader : IDownloader
    {
        public string GetContent(string url)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}
