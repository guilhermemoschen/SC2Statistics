using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Microsoft.Practices.EnterpriseLibrary.Validation;

using NHibernate.Mapping.ByCode.Impl;

using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Parser;
using SC2LiquipediaStatistics.Utilities.Web;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.SC2Domain.Repository;

using Match = SC2Statistics.SC2Domain.Model.Match;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace SC2Statistics.SC2Domain.Service
{
    public class ParseService : IParseService
    {
        #region REGEX

        public const string ValidateLiquipediaUrl = @"^(https?\:\/\/)?wiki\.teamliquid\.net\/starcraft2\/.+";

        public const string FindIfIsVodRow = @"[Vv]od\d?\.png";

        public const string FindIfIsGamesRow = "<div";

        #endregion

        public IDownloader Downloader { get; protected set; }
        public ILogger Logger { get; protected set; }
        public IPlayerRespository PlayerRespository { get; protected set; }
        public IEventRepository EventRepository { get; protected set; }

        public ParseService(IPlayerRespository playerRespository, IEventRepository eventRepository, IDownloader downloader, ILogger logger)
        {
            PlayerRespository = playerRespository;
            EventRepository = eventRepository;
            Downloader = downloader;
            Logger = logger;
        }

        #region Event
        public Event GetSC2Event(string url)
        {
            return GetSC2Event(url, null);
        }

        private Event GetSC2Event(string url, string html)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (!Regex.IsMatch(url, ValidateLiquipediaUrl))
            {
                throw new ValidationException("The URL is not a valid Liquipedia address.");
            }

            if (!Regex.IsMatch(url, "https?://"))
            {
                url = string.Format("http://{0}", url);
            }

            if (html == null)
                html = Download(url);

            Logger.Info("Parsing {0}", url);
            var timestamp = DateTime.Now.TimeOfDay;

            var sc2Event = ParseEvent(url, html);

            if (!sc2Event.IsValid)
                throw new ValidationException("The event is invalid.", sc2Event.ValidationResults);

            timestamp = DateTime.Now.TimeOfDay - timestamp;
            Logger.Info("Finished in {0:0.000} seconds", timestamp.TotalSeconds);

            return sc2Event;
        }

        public Event GetSC2EventWithSubEvents(string url)
        {
            var mainPageContent = Download(url);
            var sc2Event = GetSC2Event(url, mainPageContent);
            var subEventsContents = GetSubEventsContents(url, mainPageContent);

            foreach (var subEventContent in subEventsContents)
            {
                Event subEvent;
                try
                {
                    subEvent = GetSC2Event(subEventContent.Key, subEventContent.Value);
                }
                catch (ValidationException)
                {
                    Logger.Info("Invalid Sub Event {0}", subEventContent.Key);
                    continue;
                }

                if (!subEvent.Matches.Any())
                {
                    Logger.Info("{0} doesn't have any match.", subEvent.Name);
                }
                else
                {
                    sc2Event.AddSubEvent(subEvent);
                }
            }

            return sc2Event;
        }

        private string Download(string url)
        {
            if (!Regex.IsMatch(url, "https?://"))
                url = "http://" + url;

            Logger.Info("Downloading {0}", url);
            var timestamp = DateTime.Now.TimeOfDay;
            var html = Downloader.GetContent(new Uri(url));
            timestamp = DateTime.Now.TimeOfDay - timestamp;
            Logger.Info("Finished in {0:0.000} seconds", timestamp.TotalSeconds);
            return html;
        }

        private IDictionary<string, string> GetSubEventsContents(string mainEventUrl, string mainContent)
        {
            var subPages = new Dictionary<string, string>();
            var extraPagesUrls = new List<string>();
            var pendingExtraPagesUrl = GetSubPagesUrls(mainEventUrl, mainContent);

            while (pendingExtraPagesUrl.Any())
            {
                var url = pendingExtraPagesUrl.First();

                if (extraPagesUrls.Contains(url))
                {
                    pendingExtraPagesUrl.Remove(url);
                    continue;
                }

                extraPagesUrls.Add(url);

                var content = Download(url);
                subPages.Add(url, content);

                foreach (var page in GetSubPagesUrls(url, content))
                {
                    pendingExtraPagesUrl.Add(page);
                }
            }

            return subPages;
        }

        public Event ParseEvent(string url, string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var contentNode = htmlDocument.GetElementbyId("bodyContent");

            var infoBoxNode = contentNode.SelectSingleSubNode("//div[@class='fo-nttax-infobox wiki-bordercolor-light']");

            Event sc2Event;

            if (infoBoxNode != null)
            {
                sc2Event = new Event
                {
                    Name = GetEventName(infoBoxNode),
                    StartDate = GetEventStartDate(infoBoxNode),
                    EndDate = GetEventEndDate(infoBoxNode),
                    Expansion = GetEventExpansion(infoBoxNode),
                    LiquipediaReference = url,
                    LiquipediaTier = GetEventTier(infoBoxNode),
                    PrizePool = GetEventPrizePool(infoBoxNode),
                };
            }
            else
            {
                var headerNode = htmlDocument.DocumentNode.SelectSingleNode("//h1");

                if (headerNode == null)
                    return null;

                sc2Event = new Event
                {
                    Name = headerNode.GetCleanedInnerText(),
                    LiquipediaReference = url,
                };
            }

            foreach (var match in ParseMatches(contentNode))
            {
                sc2Event.AddMatch(match);
            }

            return sc2Event;
        }

        private string GetEventPrizePool(HtmlNode infoBoxNode)
        {
            var node = infoBoxNode
                .SelectSingleSubNode("//div[text()='Prize pool:' or text()='Prize:']");

            if (node == null)
                return null;

            return node
                .ParentNode
                .SelectSingleNode("div[2]")
                .GetCleanedInnerText();
        }

        private LiquipediaTier GetEventTier(HtmlNode infoBoxNode)
        {
            var node = infoBoxNode
                .SelectSingleSubNode("//div[text()='Liquipedia Tier:' or text()='Tier:']");

            if (node == null)
                return LiquipediaTier.Undefined;

            var result = node
                .ParentNode
                .SelectSingleNode("div[2]")
                .GetCleanedInnerText();

            if (result.Contains("Premier"))
                return LiquipediaTier.Premier;

            if (result.Contains("Major"))
                return LiquipediaTier.Major;

            if (result.Contains("Minor"))
                return LiquipediaTier.Minor;

            if (result.Contains("Team Events"))
                return LiquipediaTier.TeamEvents;

            if (result.Contains("Montly"))
                return LiquipediaTier.Montly;

            if (result.Contains("Weekly"))
                return LiquipediaTier.Weekly;

            if (result.Contains("Show Match"))
                return LiquipediaTier.ShowMatches;

            if (result.Contains("Female"))
                return LiquipediaTier.FemaleOnly;

            if (result.Contains("Misc"))
                return LiquipediaTier.Misc;

            return LiquipediaTier.Undefined;
        }

        private Expansion GetEventExpansion(HtmlNode infoBoxNode)
        {
            var node = infoBoxNode
                .SelectSingleSubNode("//div[text()='Game Version:']");

            if (node == null)
                return Expansion.Undefined;

            var result = node
                .ParentNode
                .SelectSingleNode("div[2]")
                .GetCleanedInnerText();

            if (result.Contains("Legacy of the Void"))
                return Expansion.LegacyOfTheVoid;

            if (result.Contains("Heart Of The Swarm"))
                return Expansion.HeartOfTheSwarm;

            if (result.Contains("Wings Of Liberty"))
                return Expansion.WingsOfLiberty;

            return Expansion.Undefined;
        }

        private DateTime? GetEventEndDate(HtmlNode infoBoxNode)
        {
            var node = infoBoxNode
                .SelectSingleSubNode("//div[text()='End Date:' or text()='Date:']");

            if (node == null)
                return null;

            var result = node
                .ParentNode
                .SelectSingleNode("div[2]")
                .GetCleanedInnerText();

            var endDate = Regex.Match(result, @"\d{4}-\d{2}-\d{2}").Value;

            DateTime date;
            if (DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            return null;
        }

        private DateTime? GetEventStartDate(HtmlNode infoBoxNode)
        {
            var node = infoBoxNode
                .SelectSingleSubNode("//div[text()='Start Date:' or text()='Date:']");

            if (node == null)
                return null;

            var result = node
                .ParentNode
                .SelectSingleNode("div[2]")
                .GetCleanedInnerText();

            var startDate = Regex.Match(result, @"\d{4}-\d{2}-\d{2}").Value;

            DateTime date;
            if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            return null;
        }

        private string GetEventName(HtmlNode infoBoxNode)
        {
            var node = infoBoxNode
                .SelectSingleSubNode("//div[@class='infobox-header wiki-backgroundcolor-light']");

            if (node == null)
                return null;

            return node
                .GetCleanedInnerText()
                .Replace("[e]", string.Empty)
                .Replace("[h]", string.Empty);
        }

        #endregion

        #region Matches

        public IEnumerable<Match> ParseMatches(HtmlNode contentNode)
        {
            var matches = new List<Match>();

            foreach (var groupTable in GetGroupsTables(contentNode))
            {
                foreach (var match in ConvertGroupTableToMatches(groupTable))
                {
                    matches.Add(match);
                }
            }

            foreach (var bracketGame in GetBrackets(contentNode))
            {
                var bracketMatchesPerRound = new List<IEnumerable<Match>>();
                foreach (var column in GetBracketGamesPerColumn(bracketGame))
                {
                    var matchesPerRound = ConvertBracketGamesPerRound(column);
                    if (matchesPerRound != null)
                        bracketMatchesPerRound.Add(matchesPerRound);
                }

                var allBracketMatches = DefineNextMatchForBracketMatches(bracketMatchesPerRound);
                if (allBracketMatches != null && allBracketMatches.Any())
                {
                    matches.AddRange(allBracketMatches);
                }
            }

            return matches;
        }

        public IEnumerable<Match> ConvertBracketGamesPerRound(HtmlNode columnNode)
        {
            var matches = new Collection<Match>();

            var sameRoundGames = new List<HtmlNode>();
            var bracketGamesNodes = columnNode.SelectSubNodes("//div[@class='bracket-game']");
            if (bracketGamesNodes == null)
                return null;

            sameRoundGames.AddRange(bracketGamesNodes.Where(IsBracketGameNodeValid));

            var columnHeader = columnNode.SelectSingleSubNode("//div[@class='bracket-header']").GetCleanedInnerText();

            foreach (var matchNode in sameRoundGames)
            {
                var match = ConvertBracketGameToMatch(matchNode, ConvertStringToBracketRound(columnHeader));
                if (match != null)
                    matches.Add(match);
            }

            return matches;
        }

        private BracketRound ConvertStringToBracketRound(string text)
        {
            if (string.IsNullOrEmpty(text))
                return BracketRound.Undefined;

            text = text.Trim();

            if (Regex.IsMatch(text, @"round.*?128", RegexOptions.IgnoreCase))
                return BracketRound.RoundOf128;

            if (Regex.IsMatch(text, @"round.*?64", RegexOptions.IgnoreCase))
                return BracketRound.RoundOf64;

            if (Regex.IsMatch(text, @"round.*?32", RegexOptions.IgnoreCase))
                return BracketRound.RoundOf32;

            if (Regex.IsMatch(text, @"round.*?16", RegexOptions.IgnoreCase))
                return BracketRound.RoundOf16;

            if (Regex.IsMatch(text, @"round.*?8", RegexOptions.IgnoreCase) || Regex.IsMatch(text, @"quarterfinals", RegexOptions.IgnoreCase))
                return BracketRound.RoundOf8;

            if (Regex.IsMatch(text, @"semifinals", RegexOptions.IgnoreCase))
                return BracketRound.RoundOf4;

            if (Regex.IsMatch(text, @"winner.*?final", RegexOptions.IgnoreCase))
                return BracketRound.WinnersFinals;

            if (Regex.IsMatch(text, @"loser.*?final", RegexOptions.IgnoreCase))
                return BracketRound.LosersFinals;

            if (Regex.IsMatch(text, @"grand.*?final", RegexOptions.IgnoreCase) || Regex.IsMatch(text, @"^final", RegexOptions.IgnoreCase))
                return BracketRound.GrandFinals;

            if (Regex.IsMatch(text, @"loser.*?round.*?1$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound1;

            if (Regex.IsMatch(text, @"loser.*?round.*?2$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound2;

            if (Regex.IsMatch(text, @"loser.*?round.*?3$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound3;

            if (Regex.IsMatch(text, @"loser.*?round.*?4$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound4;

            if (Regex.IsMatch(text, @"loser.*?round.*?5$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound5;

            if (Regex.IsMatch(text, @"loser.*?round.*?6$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound6;

            if (Regex.IsMatch(text, @"loser.*?round.*?7$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound7;

            if (Regex.IsMatch(text, @"loser.*?round.*?8$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound8;

            if (Regex.IsMatch(text, @"loser.*?round.*?9$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound9;

            if (Regex.IsMatch(text, @"loser.*?round.*?10$", RegexOptions.IgnoreCase))
                return BracketRound.LoeserRound10;

            if (Regex.IsMatch(text, @"loser.*?semifinals$", RegexOptions.IgnoreCase))
                return BracketRound.LoserSemifinals;

            if (Regex.IsMatch(text, @"from.*?winner.*?bracket", RegexOptions.IgnoreCase))
                return BracketRound.FromWinnerBracket;

            if (Regex.IsMatch(text, @"from.*?loser.*?bracket", RegexOptions.IgnoreCase))
                return BracketRound.FromLoserBracket;

            return BracketRound.Undefined;
        }

        public IEnumerable<Match> DefineNextMatchForBracketMatches(IList<IEnumerable<Match>> matchesPerColumn)
        {
            // define next matches
            for (var round = 0; round < matchesPerColumn.Count; round++)
            {
                if (round + 1 >= matchesPerColumn.Count)
                    break;

                foreach (var match in matchesPerColumn[round])
                {
                    match.NextMatch = FindNextBracketMatch(match.Winner, matchesPerColumn[round + 1]);
                }
            }

            return matchesPerColumn.SelectMany(x => x);
        }

        private Match FindNextBracketMatch(Player winner, IEnumerable<Match> nextRound)
        {
            return nextRound
                .FirstOrDefault(match => match.Player1.Id == winner.Id || match.Player2.Id == winner.Id);
        }

        private Match ConvertBracketGameToMatch(HtmlNode bracketGameNode, BracketRound round)
        {
            var rows = bracketGameNode.SelectSubNodes("//div[contains(@class, 'bracket-cell')]");

            var player1Row = rows[0];
            var player2Row = rows[1];

            if (!IsBracketMatchFinished(player1Row, player2Row))
                return null;

            var player1Name = GetBracketGamePlayerName(player1Row);
            var player1Score = GetBracketGamePlayerScore(player1Row);
            var player2Name = GetBracketGamePlayerName(player2Row);
            var player2Score = GetBracketGamePlayerScore(player2Row);

            if (string.IsNullOrEmpty(player1Name) || string.IsNullOrEmpty(player2Name) ||
                string.IsNullOrEmpty(player1Score) || string.IsNullOrEmpty(player2Score))
                return null;

            var match = Match.CreateBracketMatch(
                null,
                PlayerRespository.FindOrCreate(player1Name), 
                player1Score, 
                ConvertToRace(player1Row.InnerHtml),
                PlayerRespository.FindOrCreate(player2Name),
                player2Score,
                ConvertToRace(player2Row.InnerHtml),
                round,
                null
            );

            return match.Winner == null ? null : match;
        }

        private IEnumerable<HtmlNode> GetBrackets(HtmlNode contentNode)
        {
            var bracketNodes = contentNode.SelectSubNodes("//div[@class='bracket']");
            return bracketNodes ?? Enumerable.Empty<HtmlNode>();
        }

        private IEnumerable<HtmlNode> GetBracketGamesPerColumn(HtmlNode bracketNode)
        {
            return bracketNode.SelectSubNodes("//div[@class='bracket-column']");
        }

        private bool IsBracketGameNodeValid(HtmlNode htmlNode)
        {
            return !htmlNode.InnerHtml.Contains("bracket-player-middle") &&
                !htmlNode.InnerHtml.Contains("<div class=\"bracket-score\" style=\"width:21px;\">Q</div>");
        }

        private bool IsBracketMatchFinished(HtmlNode player1Node, HtmlNode player2Node)
        {
            return player1Node.OuterHtml.Contains("font-weight:bold") ||
                   player2Node.OuterHtml.Contains("font-weight:bold");
        }

        private string GetBracketGamePlayerName(HtmlNode playerNode)
        {
            var nameNode = playerNode.SelectSingleSubNode("//span");
            if (nameNode == null)
                return null;

            return nameNode.GetCleanedInnerText();
        }

        private string GetBracketGamePlayerScore(HtmlNode playerNode)
        {
            var scoreNode = playerNode.SelectSingleSubNode("//div[@class='bracket-score']");
            if (scoreNode == null)
                return null;

            return scoreNode.GetCleanedInnerText();
        }

        private IEnumerable<HtmlNode> GetGroupsTables(HtmlNode contentNode)
        {
            var groups = contentNode.SelectSubNodes("//table[@class='oldtable table table-bordered grouptable' and contains(@style, 'width: 300px')]");
            if (groups == null)
                return Enumerable.Empty<HtmlNode>();

            return groups.Select(x => x.ParentNode.ParentNode);
        }

        private IEnumerable<Match> ConvertGroupTableToMatches(HtmlNode groupNode)
        {
            if (groupNode == null)
                return Enumerable.Empty<Match>();

            var matches = new List<Match>();

            var trs = groupNode.SelectSubNodes("/table/tr");

            string groupName = null;
            var groupNameNode = groupNode.SelectSingleSubNode("//table[contains(@class, 'grouptable')]//th");
            if (groupNameNode != null)
                groupName = groupNameNode.GetCleanedInnerText();

            for (var i = 1; i < trs.Count; i++)
            {
                var matchRow = trs[i];
                HtmlNode gamesRow = null;
                HtmlNode vodsRow = null;

                for (var j = i + 1; j < trs.Count; j++)
                {
                    var nextRow = trs[j];
                    if (IsGamesRow(nextRow.InnerHtml))
                    {
                        gamesRow = nextRow;
                        i++;
                    }
                    else if (IsVodRow(nextRow.InnerHtml))
                    {
                        vodsRow = nextRow;
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                var match = ConvertGroupGameToMatch(groupName, matchRow, gamesRow, vodsRow);
                if (match != null)
                    matches.Add(match);
            }

            return matches;
        }

        private bool IsVodRow(string row)
        {
            return Regex.IsMatch(row, FindIfIsVodRow);
        }

        private bool IsGamesRow(string html)
        {
            return html.Contains(FindIfIsGamesRow);
        }

        private Match ConvertGroupGameToMatch(string groupName, HtmlNode matchNode, HtmlNode gamesNode, HtmlNode vodsNode)
        {
            if (matchNode == null)
                return null;

            if (!IsGroupGameMatchValid(matchNode))
                return null;

            var player1Name = GetGroupGamePlayer1Name(matchNode);
            var player1Score = GetGroupGamePlayer1Score(matchNode);
            var player2Name = GetGroupGamePlayer2Name(matchNode);
            var player2Score = GetGroupGamePlayer2Score(matchNode);

            if (string.IsNullOrEmpty(player1Name) || string.IsNullOrEmpty(player2Name)
                || string.IsNullOrEmpty(player1Score) || string.IsNullOrEmpty(player2Score))
                return null;

            var player1 = PlayerRespository.FindOrCreate(player1Name);
            var player2 = PlayerRespository.FindOrCreate(player2Name);

            IEnumerable<Game> games = null;
            if (gamesNode != null)
            {
                games = ConvertGamesRow(gamesNode, player1, player2);
            }

            if (vodsNode != null)
            {
                // TODO
            }

            var match = Match.CreateGroupMatch(
                groupName,
                null,
                player1,
                player1Score,
                GetPlayer1Race(matchNode),
                player2,
                player2Score,
                GetPlayer2Race(matchNode),
                games
            );

            return match.Winner == null ? null : match;
        }

        private string GetGroupGamePlayer1Score(HtmlNode matchNode)
        {
            var node = matchNode.SelectSingleNode("td[not(@class = 'matchlistslot')][1]");
            return node != null ? node.GetCleanedInnerText() : null;
        }

        private string GetGroupGamePlayer2Score(HtmlNode matchNode)
        {
            var node = matchNode.SelectSingleNode("td[not(@class = 'matchlistslot')][2]");
            return node != null ? node.GetCleanedInnerText() : null;
        }

        private string GetGroupGamePlayer1Name(HtmlNode matchNode)
        {
            var node = matchNode.SelectSingleNode("td[@class='matchlistslot'][1]");
            if (node == null)
                return null;

            return node.GetCleanedInnerText();
        }

        private string GetGroupGamePlayer2Name(HtmlNode matchNode)
        {
            var node = matchNode.SelectSingleNode("td[@class='matchlistslot'][2]");
            if (node == null)
                return null;

            return node.GetCleanedInnerText();
        }

        private bool IsGroupGameMatchValid(HtmlNode matchNode)
        {
            return matchNode.InnerHtml.Contains("font-weight:bold");
        }

        private IList<Game> ConvertGamesRow(HtmlNode gamesNode, Player player1, Player player2)
        {
            var games = new List<Game>();
            var mapsNodes = gamesNode.SelectSubNodes("/td/div");

            foreach (var mapNode in mapsNodes)
            {
                var mapName = mapNode.SelectSingleNode("a").GetCleanedInnerText();
                var winnerIndex = mapNode.SelectSubNodes("//div[contains(@style, 'float:right')]/img") == null ? 1 : 2;

                games.Add(new Game
                {
                    Map = mapName,
                    Number = games.Count + 1,
                    Winner = winnerIndex == 1 ? player1 : player2,
                });
            }

            return games;
        }

        private Race GetPlayer1Race(HtmlNode matchNode)
        {
            var td = matchNode.SelectSingleSubNode("//td[1]").InnerHtml;
            return ConvertToRace(td);
        }

        private Race GetPlayer2Race(HtmlNode matchNode)
        {
            var td = matchNode.SelectSingleSubNode("//td[last()]").InnerHtml;
            return ConvertToRace(td);
        }

        private Race ConvertToRace(string input)
        {
            if (input.Contains("Zerg") || input.Contains("background:rgb(242,184,184)"))
                return Race.Zerg;

            if (input.Contains("Protoss") || input.Contains("background:rgb(184,242,184)"))
                return Race.Protoss;

            if (input.Contains("Terran") || input.Contains("background:rgb(184,184,242)"))
                return Race.Terran;

            if (input.Contains("Random") || input.Contains("background:rgb(242,232,184)"))
                return Race.Random;

            return Race.Undefined;
        }

        public IList<string> GetSubPagesUrls(string pageUrl, string pageContent)
        {
            var subPagesUrls = new List<string>();

            var mainPagePath = Regex.Replace(pageUrl, @".*?\/starcraft2\/", string.Empty).Replace("/", "\\/");
            var pattern = string.Format(@"\<a href=.\/starcraft2\/{0}\/[\w\/]*?(?=\" + "\"" + ")", mainPagePath);
            var urls = Regex.Matches(pageContent, pattern).Cast<RegexMatch>().Select(x => x.Value);

            foreach (var url in urls)
            {
                var cleanUrl = Regex.Replace(url, ".*?\\\"\\/", string.Empty);
                cleanUrl = string.Format("http://wiki.teamliquid.net/{0}", cleanUrl);

                if (!subPagesUrls.Contains(cleanUrl))
                    subPagesUrls.Add(cleanUrl);
            }

            return subPagesUrls;
        }

        #endregion
    }
}
