using System.Collections.Generic;

using SC2Statistics.StatisticDomain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.StatisticDomain.Database.Repository
{
    public interface IEventRepository : IRepositoryBase<Event>
    {
        IList<Event> FindEventsByPlayer(long playerId);

        Event FindByReference(string url);

        Event FindByAligulacId(int aligulacId);
    }
}