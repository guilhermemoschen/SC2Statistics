using System.Collections.Generic;
using System.Threading.Tasks;

using SC2LiquipediaStatistics.DesktopClient.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Service
{
    public interface IEventsListService
    {
        Task<IEnumerable<EventItem>> GetEventItems();
    }
}