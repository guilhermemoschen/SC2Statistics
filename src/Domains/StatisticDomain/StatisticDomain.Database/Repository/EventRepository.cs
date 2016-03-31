using System.Collections.Generic;
using System.Linq;

using NHibernate.Linq;

using SC2Statistics.StatisticDomain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.StatisticDomain.Database.Repository
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public IList<Event> FindEventsByPlayer(long playerId)
        {
            return Session.Query<Event>()
                .Where(x => x.Matches.Any(y => y.Player1.Id == playerId || y.Player2.Id == playerId))
                .ToList();
        }

        public Event FindByReference(string url)
        {
            return Session.Query<Event>()
                .FirstOrDefault(x => x.AligulacReference == url);
        }

        public Event FindByAligulacId(int aligulacId)
        {
            return Session.Query<Event>()
                .FirstOrDefault(x => x.AligulacId == aligulacId);
        }
    }
}
