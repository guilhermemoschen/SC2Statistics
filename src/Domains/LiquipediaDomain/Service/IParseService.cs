using System.Collections.Generic;
using System.Threading.Tasks;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Service
{
    public interface IParseService
    {
        Event GetSC2Event(string url);
        Event GetSC2EventWithSubEvents(string url);
    }
}