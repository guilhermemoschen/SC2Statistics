using System.Collections.Generic;
using System.Linq;

using NHibernate.Linq;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public IList<Event> GetAllEvents()
        {
            return Session.Query<Event>().ToList();
        }
    }
}
