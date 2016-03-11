using System;
using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.Utilities.Web
{
    public interface IDownloader
    {
        string GetContent(Uri url);
    }
}