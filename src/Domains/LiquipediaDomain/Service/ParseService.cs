using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Practices.EnterpriseLibrary.Validation;

using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Log;
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

        public const string ValidateLiquipediaLink = @"^(https?\:\/\/)?wiki\.teamliquid\.net\/starcraft2\/.+";

        public const string FindPrizePool = @"(?<=Prize pool:\<\/div\>\s).*?(?=\<\/)";

        public const string FindLiquepediaTier = @"Liquipedia Tier:\<\/div\>[\s\S]*?\<\/";

        public const string FindGameVersion = @"Game Version:\<\/div\>[\s\S]*?\<\/";

        public const string FindEndDate = @"(?<=End Date:\<\/div\>)[\s\S]*?(?=\<\/)";

        public const string FindStartDate = @"(?<=Start Date:\<\/div\>)[\s\S]*?(?=\<\/)";

        public const string FindEventName = @"(?<=\>).*(?=\<\/h1)";

        public const string FindGroupsTables = @"\<table.+matchlist[\s\S]*?<\/table\>";

        public const string FindGroupsTablesRows = @"(\<tr[^>]*?\>)\s?<td([\s\S](?!<tr))*\<\/tr\>";

        public const string FindIfIsVodRow = @"[Vv]od\d\.png";

        public const string FindIfIsGamesRow = "<div";

        public const string FindPlayersNamesFromGroupMatchRow = @"\w+(?=<\/span)";

        public const string FindIfBracketMatchIsFinished = @"\<div.*?bracket-cell.*?font-weight:bold";

        public const string FindIfIsGroupMatchFinished = @"\<td.*?matchlistslot.*?font-weight:bold";

        public const string FindIfPlayerOneWonTheMap = @"left.*?\>.*?GreenCheck.*?right";

        public const string FindMapName = @"[\s\S]*?\<div.*?href.*?\>[\s]?";

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

            if (!Regex.IsMatch(url, ValidateLiquipediaLink))
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
            var sc2Event = new Event
            {
                Name = GetEventName(html),
                StartDate = GetEventStartDate(html),
                EndDate = GetEventEndDate(html),
                Expansion = GetEventExpansion(html),
                LiquipediaReference = url,
                LiquipediaTier = GetEventTier(html),
                PrizePool = GetEventPrizePool(html)
            };

            foreach (var match in ParseMatches(url, html))
            {
                sc2Event.AddMatch(match);
            }

            return sc2Event;
        }

        private string GetEventPrizePool(string html)
        {
            var result = Regex.Match(html, FindPrizePool);
            if (!result.Success)
                return null;

            var prizePool = Regex.Replace(result.Value, @"\<.*?\>", string.Empty);
            return string.IsNullOrEmpty(prizePool) ? null : prizePool;
        }

        private LiquipediaTier GetEventTier(string html)
        {
            var result = Regex.Match(html, FindLiquepediaTier);
            if (!result.Success)
                return LiquipediaTier.Undefined;

            if (result.Value.Contains("Premier"))
                return LiquipediaTier.Premier;

            if (result.Value.Contains("Major"))
                return LiquipediaTier.Major;

            if (result.Value.Contains("Minor"))
                return LiquipediaTier.Minor;

            if (result.Value.Contains("Team Events"))
                return LiquipediaTier.TeamEvents;

            if (result.Value.Contains("Montly"))
                return LiquipediaTier.Montly;

            if (result.Value.Contains("Weekly"))
                return LiquipediaTier.Weekly;

            if (result.Value.Contains("Show Match"))
                return LiquipediaTier.ShowMatches;

            if (result.Value.Contains("Female"))
                return LiquipediaTier.FemaleOnly;

            if (result.Value.Contains("Misc"))
                return LiquipediaTier.Misc;

            return LiquipediaTier.Undefined;
        }

        private Expansion GetEventExpansion(string html)
        {
            var result = Regex.Match(html, FindGameVersion);
            if (!result.Success)
                return Expansion.Undefined;

            if (result.Value.Contains("Legacy of the Void"))
                return Expansion.LegacyOfTheVoid;

            if (result.Value.Contains("Heart Of The Swarm"))
                return Expansion.HeartOfTheSwarm;

            if (result.Value.Contains("Wings Of Liberty"))
                return Expansion.WingsOfLiberty;

            return Expansion.Undefined;
        }

        private DateTime? GetEventEndDate(string html)
        {
            var result = Regex.Match(html, FindEndDate);
            if (!result.Success)
                return null;

            var endDate = Regex.Match(result.Value, @"\d{4}-\d{2}-\d{2}").Value;

            DateTime date;
            if (DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            return null;
        }

        private DateTime? GetEventStartDate(string html)
        {
            var result = Regex.Match(html, FindStartDate);
            if (!result.Success)
                return null;

            var startDate = Regex.Match(result.Value, @"\d{4}-\d{2}-\d{2}").Value;

            DateTime date;
            if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            return null;
        }

        private string GetEventName(string html)
        {
            var match = Regex.Match(html, FindEventName);
            return match.Success ? match.Value : null;
        }

        #endregion

        #region Matches

        public IEnumerable<Match> ParseMatches(string pageUrl, string pageContent)
        {
            var matches = new List<Match>();

            foreach (var groupTable in GetGroupsTables(pageContent))
            {
                foreach (var match in ConvertGroupTableToMatches(groupTable))
                {
                    match.LiquipediaReference = pageUrl;
                    matches.Add(match);
                }
            }

            foreach (var bracketGame in GetBracketGames(pageContent))
            {
                var match = ConvertBacketGameToMatch(bracketGame);
                if (match != null)
                {
                    match.LiquipediaReference = pageUrl;
                    matches.Add(match);
                }
            }

            return matches;
        }

        private Match ConvertBacketGameToMatch(string bracketGameHtml)
        {
            if (bracketGameHtml.Contains("<div class=\"bracket-score\" style=\"width:21px;\">Q</div>"))
                return null;

            var rows = Regex.Split(bracketGameHtml, "\\<div.*?bracket-cell");

            var player1Row = rows[1];
            var player2Row = rows[2];

            if (!IsBracketMatchFinished(bracketGameHtml))
                return null;

            var player1Name = Regex.Replace(player1Row, "[\\s\\S]*?bracket-player[\\s\\S]*?\\<span[\\s\\S]*?\\>", string.Empty);
            player1Name = Regex.Replace(player1Name, "\\<[\\s\\S]*", string.Empty);
            var player1Score = Regex.Replace(player1Row, "[\\s\\S]*?bracket-score[\\s\\S]*?\\>", string.Empty);
            player1Score = Regex.Replace(player1Score, "\\<[\\s\\S]*", string.Empty);

            var player2Name = Regex.Replace(player2Row, "[\\s\\S]*?bracket-player[\\s\\S]*?\\<span[\\s\\S]*?\\>", string.Empty);
            player2Name = Regex.Replace(player2Name, "\\<[\\s\\S]*", string.Empty);
            var player2Score = Regex.Replace(player2Row, "[\\s\\S]*?bracket-score[\\s\\S]*?\\>", string.Empty);
            player2Score = Regex.Replace(player2Score, "\\<[\\s\\S]*", string.Empty);

            if (string.IsNullOrEmpty(player1Name) || string.IsNullOrEmpty(player2Name))
                return null;

            var match = new Match(
                null,
                null,
                PlayerRespository.FindOrCreate(player1Name),
                player1Score,
                ConvertToRace(player1Row),
                PlayerRespository.FindOrCreate(player2Name),
                player2Score,
                ConvertToRace(player2Row)
            );

            return match.Winner == null ? null : match;
        }

        private IEnumerable<string> GetBracketGames(string html)
        {
            html = RemoveSinglePlayerBracket(html);
            var regex = new Regex("\\<div.*?bracket-game[\\s\\S]*?bracket-player-bottom[\\s\\S]*?bracket-score[\\s\\S]*?\\<\\/div\\>");
            var result = regex.Matches(html);
            return result.Cast<RegexMatch>().Select(x => x.Value);
        }

        private string RemoveSinglePlayerBracket(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var cleanHtml = html;

            var singleBracketIndex = cleanHtml.IndexOf("<div class=\"bracket-player-middle\"");

            while (singleBracketIndex != -1)
            {
                var bracketGameIndex = cleanHtml.LastIndexOf("<div class=\"bracket-game\">", singleBracketIndex);
                cleanHtml = RemoveBracketGame(bracketGameIndex, cleanHtml);
                singleBracketIndex = cleanHtml.IndexOf("<div class=\"bracket-player-middle\"");
            }

            return cleanHtml;
        }

        private string RemoveBracketGame(int bracketGameIndex, string html)
        {
            var openDiv = "<div";
            var closeDiv = "</div>";
            var divDepth = 0;

            for (var i = bracketGameIndex + 1; i < html.Length; i++)
            {
                if (html.Substring(i, openDiv.Length) == openDiv)
                    divDepth++;

                if (html.Substring(i, closeDiv.Length) == closeDiv)
                    divDepth--;

                if (divDepth < 0)
                    return html.Remove(bracketGameIndex, i - bracketGameIndex + closeDiv.Length);
            }

            return html;
        }

        private IEnumerable<string> GetGroupsTables(string html)
        {
            var result = Regex.Matches(html, FindGroupsTables);
            return result.Cast<RegexMatch>().Select(x => x.Value);
        }

        private IEnumerable<Match> ConvertGroupTableToMatches(string groupTable)
        {
            if (groupTable == null)
                return new Match[0];

            var rows = Regex.Matches(groupTable, FindGroupsTablesRows);

            var matches = new List<Match>();

            for (var i = 0; i < rows.Count; i++)
            {
                var matchRow = rows[i].Value;
                string gamesRow = null;
                string vodsRow = null;

                for (var j = i + 1; j < rows.Count; j++)
                {
                    var nextRow = rows[j].Value;
                    if (IsGamesRow(nextRow))
                    {
                        gamesRow = nextRow;
                    }
                    else if (IsVodRow(nextRow))
                    {
                        vodsRow = nextRow;
                    }
                    else
                    {
                        break;
                    }
                }

                var match = ConvertGroupGameToMatch(matchRow, gamesRow, vodsRow);
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

        private Match ConvertGroupGameToMatch(string matchHtml, string gamesHtml, string vodsHtml)
        {
            if (matchHtml == null)
                return null;

            if (!IsGroupMatchFinished(matchHtml))
                return null;

            var names = Regex.Matches(matchHtml, FindPlayersNamesFromGroupMatchRow);
            if (names.Count != 2)
            {
                Logger.Info("Could not parse the players names in a match row");
                return null;
            }

            var scores = Regex.Matches(matchHtml, @"\w+(?=<\/td)");
            if (scores.Count != 2)
            {
                Logger.Info("Could not parse the scores in a match row");
                return null;
            }

            var player1Name = names[0].Value;
            var player1Score = scores[0].Value;
            var player2Score = scores[1].Value;
            var player2Name = names[1].Value;

            if (string.IsNullOrEmpty(player1Name) || string.IsNullOrEmpty(player2Name))
                return null;

            var match = new Match(
                null,
                null,
                PlayerRespository.FindOrCreate(player1Name),
                player1Score,
                GetPlayer1Race(matchHtml),
                PlayerRespository.FindOrCreate(player2Name),
                player2Score,
                GetPlayer2Race(matchHtml)
            );

            if (match.Winner == null)
                return null;

            if (gamesHtml != null)
            {
                match.Games = ConvertGamesRow(gamesHtml, match);
            }

            if (vodsHtml != null)
            {
                // TODO
            }

            return match;
        }

        private bool IsBracketMatchFinished(string matchHtml)
        {
            return Regex.IsMatch(matchHtml, FindIfBracketMatchIsFinished);
        }

        private bool IsGroupMatchFinished(string matchHtml)
        {
            return Regex.IsMatch(matchHtml, FindIfIsGroupMatchFinished);
        }

        private IList<Game> ConvertGamesRow(string gamesRow, Match match)
        {
            var games = new List<Game>();

            var mapsRows = Regex.Split(gamesRow, @"\<\/a\>");

            foreach (var mapRow in mapsRows.Take(mapsRows.Length - 1))
            {
                var mapName = Regex.Replace(mapRow, FindMapName, string.Empty);

                games.Add(new Game
                {
                    Map = mapName,
                    Number = games.Count + 1,
                    Winner = Regex.IsMatch(mapRow, FindIfPlayerOneWonTheMap) ? match.Player1 : match.Player2,
                    Match = match
                });
            }

            return games;
        }

        private Race GetPlayer1Race(string matchRow)
        {
            var tds = Regex.Split(matchRow, "<td");
            return ConvertToRace(tds.Skip(1).First());
        }

        private Race GetPlayer2Race(string matchRow)
        {
            var tds = Regex.Split(matchRow, "<td");
            return ConvertToRace(tds.Last());
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

            var mainPagePath = Regex.Replace(pageUrl, @".*?\/starcraft2\/", string.Empty);
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
