using System.Collections.Generic;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
{
    public interface IEventRepository : IRepositoryBase<Event>
    {
        IList<Event> GetAllEvents();
    }
}