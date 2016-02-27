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
        public async Task<string> GetContent(string url)
        {
            using (var client = new WebClient())
            {
                return await client.DownloadStringTaskAsync(url);
            }
        }
    }
}
