using System.Collections.Generic;
using System.Threading.Tasks;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;
using SC2LiquipediaStatistics.Utilities.Web;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Service
{
    public interface ILiquipediaService
    {
        Task<Event> GetEvent(string mainPageEventUrl);
    }
}