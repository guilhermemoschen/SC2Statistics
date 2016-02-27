using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;
using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Web;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Service
{
    public class LiquipediaService : ILiquipediaService
    {
        public IDownloader Downloader { get; protected set; }

        public IParseService ParseService { get; protected set; }

        public ILogger Logger { get; protected set; }

        public LiquipediaService(IParseService parseService, IDownloader downloader, ILogger logger)
        {
            ParseService = parseService;
            Downloader = downloader;
            Logger = logger;
        }

        public async Task<Event> GetEvent(string mainPageEventUrl)
        {
            Logger.Info("Getting MainPage from Event {0}", mainPageEventUrl);
            var timestamp = DateTime.Now.TimeOfDay;
            var mainPageContent = await Downloader.GetContent(mainPageEventUrl);
            timestamp = DateTime.Now.TimeOfDay - timestamp;
            Logger.Info("OK {0} seconds", timestamp.TotalSeconds);

            Logger.Info("Getting ExtraPages from Event {0}", mainPageEventUrl);
            timestamp = DateTime.Now.TimeOfDay;
            var extraPagesContents = await GetExtraPagesContents(mainPageEventUrl, mainPageContent);
            timestamp = DateTime.Now.TimeOfDay - timestamp;
            Logger.Info("OK {0} seconds", timestamp.TotalSeconds);

            Logger.Info("Parsing Event {0}", mainPageEventUrl);
            timestamp = DateTime.Now.TimeOfDay;
            var sc2Event = ParseService.ParseEvent(mainPageEventUrl, mainPageContent, extraPagesContents);
            timestamp = DateTime.Now.TimeOfDay - timestamp;
            Logger.Info("OK {0} seconds", timestamp.TotalSeconds);

            return sc2Event;
        }

        private async Task<IDictionary<string, string>> GetExtraPagesContents(string mainPageEventUrl, string mainPageContent)
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

                var content = await Downloader.GetContent(url);
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
