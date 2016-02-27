using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.Utilities.Web
{
    public interface IDownloader
    {
        Task<string> GetContent(string url);
    }
}