using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;
using SC2LiquipediaStatistics.LiquipediaDomain.Repository;

using RegexMatch = System.Text.RegularExpressions.Match;
using Match = SC2LiquipediaStatistics.LiquipediaDomain.Model.Match;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Service
{
    public class ParseService : IParseService
    {
        public IPlayerRespository PlayerRespository { get; protected set; }

        public ParseService(IPlayerRespository playerRespository)
        {
            PlayerRespository = playerRespository;
        }

        #region Event
        public Event ParseEvent(string mainEventUrl, string mainEventContent, IDictionary<string, string> subEvents = null)
        {
            var sc2Event = new Event()
            {
                Name = GetEventName(mainEventContent),
                StartDate = GetEventStartDate(mainEventContent),
                EndDate = GetEventEndDate(mainEventContent),
                Expansion = GetEventExpansion(mainEventContent),
                LiquipediaReference = mainEventUrl,
                LiquipediaTier = GetEventTier(mainEventContent),
                PrizePool = GetEventPrizePool(mainEventContent),
            };

            foreach (var match in ParseMatches(mainEventUrl, mainEventContent))
            {
                sc2Event.AddMatch(match);
            }

            if (subEvents != null)
            {
                foreach (var subEventUrl in subEvents.Keys)
                {
                    foreach (var match in ParseMatches(subEventUrl, subEvents[subEventUrl]))
                    {
                        sc2Event.AddMatch(match);
                    }
                }
            }

            return sc2Event;
        }

        private string GetEventPrizePool(string mainEventContent)
        {
            var result = Regex.Match(mainEventContent, "Prize pool:\\<\\/div\\>[\\s\\S]*?\\<\\/");
            if (!result.Success)
                return null;

            var prizePool = result.Value;
            prizePool = Regex.Replace(prizePool, "[\\s\\S]*?\\\"\\>", string.Empty);
            prizePool = prizePool.Replace("</", string.Empty);
            prizePool = prizePool.Trim();

            return string.IsNullOrEmpty(prizePool) ? null : prizePool;
        }

        private LiquipediaTier GetEventTier(string mainEventContent)
        {
            var result = Regex.Match(mainEventContent, "Liquipedia Tier:\\<\\/div\\>[\\s\\S]*?\\<\\/");
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

        private Expansion GetEventExpansion(string mainEventContent)
        {
            var result = Regex.Match(mainEventContent, "Game Version:\\<\\/div\\>[\\s\\S]*?\\<\\/");
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

        private DateTime? GetEventEndDate(string mainEventContent)
        {
            var result = Regex.Match(mainEventContent, "End Date:\\<\\/div\\>[\\s\\S]*?\\<\\/");
            if (!result.Success)
                return null;

            var endDate = result.Value;
            endDate = Regex.Replace(endDate, "[\\s\\S]*?\\\"\\>", string.Empty);
            endDate = endDate.Replace("</", string.Empty);

            DateTime date;
            if (DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            return null;
        }

        private DateTime? GetEventStartDate(string mainEventContent)
        {
            var result = Regex.Match(mainEventContent, "Start Date:\\<\\/div\\>[\\s\\S]*?\\<\\/");
            if (!result.Success)
                return null;

            var startDate = result.Value;
            startDate = Regex.Replace(startDate, "[\\s\\S]*?\\\"\\>", string.Empty);
            startDate = startDate.Replace("</", string.Empty);

            DateTime date;
            if (DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }
            return null;
        }

        private string GetEventName(string mainEventContent)
        {
            var name = Regex.Match(mainEventContent, "\\<h1.*?h1>").Value;
            name = name.Replace("</h1>", string.Empty);
            return Regex.Replace(name, ".*?\\>", string.Empty);
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

            if (!IsMatchFinished(player1Row, player2Row))
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
            var regex = new Regex("\\<table class\\=\\\"matchlist.*\\>[\\s\\S]*?<\\/table\\>");
            var result = regex.Matches(html);
            return result.Cast<RegexMatch>().Select(x => x.Value);
        }

        private IEnumerable<Match> ConvertGroupTableToMatches(string groupTable)
        {
            if (groupTable == null)
                return new Match[0];

            var rows = Regex.Split(groupTable.Substring(groupTable.IndexOf("</th>")), "\\<tr.*?\\>");

            if (rows.Any())
            {
                var matches = new List<Match>();

                for (var i = 1; i < rows.Length; i++)
                {
                    var matchRow = rows[i];
                    string gamesRow = null;

                    if (i + 1 < rows.Length)
                    {
                        if (rows[i + 1].Contains("<div"))
                        {
                            gamesRow = rows[i + 1];
                            i++;
                        }
                    }

                    var match = ConvertMatchRow(matchRow, gamesRow);
                    if (match != null)
                        matches.Add(match);
                }

                return matches;
            }

            return new Match[0];
        }

        private Match ConvertMatchRow(string matchRow, string mapsRown = null)
        {
            if (matchRow == null)
                return null;

            var cells = Regex.Split(matchRow, "\\<\\/td\\>");
            var player1Name = cells[0];
            var player1Score = cells[1];
            var player2Score = cells[2];
            var player2Name = cells[3];

            if (!IsMatchFinished(player1Name, player2Name))
                return null;

            player1Name = Regex.Replace(player1Name, "\\n*\\<td.*?\\<span.*?\\>", string.Empty);
            player1Name = Regex.Replace(player1Name, "<.*", string.Empty);

            player1Score = Regex.Replace(player1Score, "\\n*\\<.*?\\>", string.Empty);

            player2Score = Regex.Replace(player2Score, "\\n*\\<.*?\\>", string.Empty);

            player2Name = Regex.Replace(player2Name, "\\n*\\<td.*?\\<span.*?\\>", string.Empty);
            player2Name = Regex.Replace(player2Name, "<.*", string.Empty);


            if (string.IsNullOrEmpty(player1Name) || string.IsNullOrEmpty(player2Name))
                return null;

            var match = new Match(
                null,
                null,
                PlayerRespository.FindOrCreate(player1Name),
                player1Score,
                ConvertToRace(cells[0]),
                PlayerRespository.FindOrCreate(player2Name),
                player2Score,
                ConvertToRace(cells[3])
            );

            if (match.Winner == null)
                return null;

            if (mapsRown != null)
            {
                match.Games = ConvertGamesRow(mapsRown, match);
            }

            return match;
        }

        private bool IsMatchFinished(string player1Html, string player2Html)
        {
            if (player1Html.Contains("font-weight:bold"))
                return true;

            if (player2Html.Contains("font-weight:bold"))
                return true;

            return false;
        }

        private IList<Game> ConvertGamesRow(string gamesRow, Match match)
        {
            var games = new List<Game>();

            var mapsRows = Regex.Split(gamesRow, "\\<\\/a\\>");
            for (var i = 0; i < mapsRows.Length - 1; i++)
            {
                var mapName = Regex.Replace(mapsRows[i], "[\\s\\S]*?\\<div.*?href.*?\\>", string.Empty);
                mapName = Regex.Replace(mapName, "\\n", string.Empty);

                games.Add(new Game()
                {
                    Map = mapName,
                    Number = i + 1,
                    Winner = Regex.IsMatch(mapsRows[i], "left.*?\\>.*?GreenCheck.*?right") ? match.Player1 : match.Player2,
                    Match = match,
                });
            }

            return games;
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

            var mainPagePath = Regex.Replace(pageUrl, ".*?\\/starcraft2\\/", string.Empty);
            var pattern = string.Format("\\<a href=\\\"\\/starcraft2\\/{0}\\/.*?\\\"", mainPagePath);
            var urls = Regex.Matches(pageContent, pattern).Cast<RegexMatch>().Select(x => x.Value);

            foreach (var url in urls)
            {
                var cleanUrl = Regex.Replace(url, ".*?\\\"\\/", string.Empty);
                cleanUrl = string.Format("http://wiki.teamliquid.net/{0}", cleanUrl.Replace("\"", string.Empty));

                if (!subPagesUrls.Contains(cleanUrl))
                    subPagesUrls.Add(cleanUrl);
            }

            return subPagesUrls;
        }
        
        #endregion
    }
}
