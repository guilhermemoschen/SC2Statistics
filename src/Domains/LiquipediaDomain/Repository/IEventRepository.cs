using System.Collections.Generic;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.SC2Domain.Repository
{
    public interface IEventRepository : IRepositoryBase<Event>
    {
        IList<Event> FindEventsByPlayer(long playerId);

        Event FindByReference(string url);

        Event FindByAligulacId(int aligulacId);
    }
}