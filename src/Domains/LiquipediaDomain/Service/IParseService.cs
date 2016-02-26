using System.Collections.Generic;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Service
{
    public interface IParseService
    {
        Event ParseEvent(string mainEventUrl, string mainEventContent, IDictionary<string, string> subEvents = null);

        IEnumerable<Match> ParseMatches(string pageUrl, string pageContent);

        IList<string> GetSubPagesUrls(string pageUrl, string pageContent);
    }
}