using System.Collections.Generic;
using System.Linq;

using NHibernate.Linq;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public IList<Event> FindMainEvents()
        {
            return Session.Query<Event>()
                .Where(x => x.MainEvent == null)
                .ToList();
        }

        public IList<Event> FindEventsByPlayer(long playerId)
        {
            return Session.Query<Event>()
                .Where(x => x.Matches.Any(y => y.Player1.Id == playerId || y.Player2.Id == playerId))
                .ToList();
        }

        public Event FindByReference(string url)
        {
            return Session.Query<Event>()
                .FirstOrDefault(x => x.LiquipediaReference.ToUpper() == url.ToUpper());
        }
    }
}
