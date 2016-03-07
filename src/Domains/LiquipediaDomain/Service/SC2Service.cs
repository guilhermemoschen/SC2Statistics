using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Web;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.SC2Domain.Repository;

namespace SC2Statistics.SC2Domain.Service
{
    public class SC2Service : ISC2Service
    {
        public IDownloader Downloader { get; protected set; }

        public IParseService ParseService { get; protected set; }

        public IEventRepository EventRepository { get; protected set; }

        public IPlayerRespository PlayerRepository { get; protected set; }

        public ILogger Logger { get; protected set; }

        public SC2Service(IParseService parseService, IEventRepository eventRepository, IPlayerRespository playerRespository, IDownloader downloader, ILogger logger)
        {
            ParseService = parseService;
            EventRepository = eventRepository;
            PlayerRepository = playerRespository;
            Downloader = downloader;
            Logger = logger;
        }

        public async Task<Event> ParseEvent(string mainPageEventUrl)
        {
            Logger.Info("Downloading Event information...");
            var timestamp = DateTime.Now.TimeOfDay;
            var mainPageContent = await Downloader.GetContent(mainPageEventUrl);
            timestamp = DateTime.Now.TimeOfDay - timestamp;
            Logger.Info("OK {0} seconds", timestamp.TotalSeconds);

            Logger.Info("Downloading Event Extra pages...");
            timestamp = DateTime.Now.TimeOfDay;
            var extraPagesContents = await GetExtraPagesContents(mainPageEventUrl, mainPageContent);
            timestamp = DateTime.Now.TimeOfDay - timestamp;
            Logger.Info("OK {0} seconds", timestamp.TotalSeconds);

            Logger.Info("Parsing Event...");
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

        public IList<Event> FindAllEvents()
        {
            using (EventRepository.Session)
            {
                return EventRepository
                    .FindAll()
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        public void CreateEvent(Event sc2Event)
        {
            using (EventRepository.Session)
            using (var scope = new TransactionScope())
            {
                EventRepository.Save(sc2Event);
                scope.Complete();
            }
        }

        public IList<Player> FindAllPlayers()
        {
            return PlayerRepository
                .FindAll()
                .OrderBy(x => x.Name)
                .ToList();
        }

        public void UpdateEvent(Event sc2Event)
        {
            if (sc2Event == null)
                throw new ArgumentNullException("sc2Event");

            using (EventRepository.Session)
            using (var scope = new TransactionScope())
            {
                EventRepository.Merge(sc2Event);
                scope.Complete();
            }
        }

        public Event LoadEvent(long eventId)
        {
            using (EventRepository.Session)
            {
                return EventRepository.Load(eventId);
            }
        }
    }
}
