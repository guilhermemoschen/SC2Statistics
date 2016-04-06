using System;
using System.IO;
using System.Net;
using System.Text;

namespace SC2Statistics.Utilities.Web
{
    public class Downloader : IDownloader
    {
        public string GetContent(Uri url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(url);
            }
        }

        public byte[] GetContentAsBytes(Uri url)
        {
            using (var client = new WebClient())
            {
                return client.DownloadData(url);
            }
        }
    }
}
