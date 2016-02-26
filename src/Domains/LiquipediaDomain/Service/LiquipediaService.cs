using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;
using SC2LiquipediaStatistics.Utilities.Web;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Service
{
    public class LiquipediaService
    {
        public IDownloader Downloader { get; protected set; }

        public IParseService ParseService { get; protected set; }

        public LiquipediaService(IParseService parseService, IDownloader downloader)
        {
            ParseService = parseService;
            Downloader = downloader;
        }

        public IList<Event> GetEvents(IEnumerable<string> mainPageEventsUrls)
        {
            return mainPageEventsUrls.Select(GetEvent).ToList();
        }

        public Event GetEvent(string mainPageEventUrl)
        {
            var mainPageContent = Downloader.GetContent(mainPageEventUrl);
            var extraPagesContents = GetExtraPagesContents(mainPageEventUrl, mainPageContent);
            return ParseService.ParseEvent(mainPageEventUrl, mainPageContent, extraPagesContents);
        }

        private IDictionary<string, string> GetExtraPagesContents(string mainPageEventUrl, string mainPageContent)
        {
            var subPages = new Dictionary<string, string>();
            var extraPagesUrls = new List<string>();
            var pendingExtraPagesUrl = ParseService.GetSubPagesUrls(mainPageEventUrl, mainPageContent);

            while (pendingExtraPagesUrl.Any())
            {
                var url = pendingExtraPagesUrl.First();

                if (extraPagesUrls.Contains(url))
                {
                    pendingExtraPagesUrl.Remove(url);
                    continue;
                }

                extraPagesUrls.Add(url);

                var content = Downloader.GetContent(url);
                subPages.Add(url, content);

                foreach (var page in ParseService.GetSubPagesUrls(url, content))
                {
                    pendingExtraPagesUrl.Add(page);
                }
            }

            return subPages;
        }
    }
}
