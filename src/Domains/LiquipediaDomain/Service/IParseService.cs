using System.Collections.Generic;
using System.Threading.Tasks;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Service
{
    public interface IParseService
    {
        Task<Event> ParseEvent(string url);
    }
}