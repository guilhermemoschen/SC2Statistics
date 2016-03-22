using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.SC2Domain.Repository;

namespace SC2Statistics.SC2Domain.Service
{
    public class StatisticsService : IStatisticsService
    {
        public IMatchRepository MatchRepository { get; protected set; }

        public StatisticsService(IMatchRepository matchRepository)
        {
            MatchRepository = matchRepository;
        }

        public PlayerStatistics GeneratePlayerStatistics(Player player, Expansion expansion)
        {
            var allMatches = MatchRepository.FindMatchesByPlayerAndExpansion(player, expansion);

            if (!allMatches.Any())
                return null;

            var matchesWon = allMatches.Where(x => x.Winner.Id == player.Id).ToList();

            var statistics = new PlayerStatistics()
            {
                Player = player,
                WinRate = (decimal)matchesWon.Count / (decimal)allMatches.Count,
                TotalMatchesPlayed = allMatches.Count,
                TotalEventsParticipated = allMatches.GroupBy(x => x.Event).Count(),
                EventsParticipated = allMatches.GroupBy(x => x.Event).Select(x => x.Key).ToList(),
            };

            var matchesXZerg = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2Race == Race.Zerg) ||
                    (x.Player2.Id == player.Id && x.Player1Race == Race.Zerg)
                )
                .ToList();

            if (matchesXZerg.Any())
            {
                var matchesWonXZerg = matchesXZerg.Where(x => x.Winner.Id == player.Id).ToList();
                statistics.WinRateXZerg = (decimal)matchesWonXZerg.Count / (decimal)matchesXZerg.Count;
                statistics.TotalMatchesPlayedAgainstZerg = matchesXZerg.Count;
            }

            var matchesXProtoss = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2Race == Race.Protoss) ||
                    (x.Player2.Id == player.Id && x.Player1Race == Race.Protoss)
                )
                .ToList();

            if (matchesXProtoss.Any())
            {
                var matchesWonXProtoss = matchesXProtoss.Where(x => x.Winner.Id == player.Id).ToList();
                statistics.WinRateXProtoss = (decimal)matchesWonXProtoss.Count / (decimal)matchesXProtoss.Count;
                statistics.TotalMatchesPlayedAgainstProtoss = matchesXProtoss.Count;
            }

            var matchesXTerran = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2Race == Race.Terran) ||
                    (x.Player2.Id == player.Id && x.Player1Race == Race.Terran)
                )
                .ToList();

            if (matchesXTerran.Any())
            {
                var matchesWonXTerran = matchesXTerran.Where(x => x.Winner.Id == player.Id).ToList();
                statistics.WinRateXTerran = (decimal)matchesWonXTerran.Count / (decimal)matchesXTerran.Count;
                statistics.TotalMatchesPlayedAgainstTerran = matchesXTerran.Count;
            }

            return statistics;
        }

        public List<List<Match>> GeneratePlayerPathbyEvent(long playerId, long eventId)
        {
            var allMatches = new List<List<Match>>();

            var matches = MatchRepository.FindMatchesByPlayerAndEvent(playerId, eventId);
            if (!matches.Any())
                return allMatches;

            var allBracketMatches = matches.Where(x => x.Format == MatchFormat.Bracket).ToList();
            

            while (allBracketMatches.Any())
            {
                var currentMatch = allBracketMatches.First();
                var bracketMatches = new List<Match>();

                bracketMatches.Add(currentMatch);
                allBracketMatches.Remove(currentMatch);

                if (currentMatch.Winner.Id != playerId)
                {
                    allMatches.Add(bracketMatches);
                    continue;
                }

                Match nextMatch;
                while ((nextMatch = allBracketMatches.FirstOrDefault()) != null)
                {
                    bracketMatches.Add(nextMatch);
                    allBracketMatches.Remove(nextMatch);

                    if (nextMatch.Winner.Id != playerId)
                    {
                        break;
                    }
                }

                allMatches.Add(bracketMatches);
            }

            var allGroupMatches = matches.Where(x => x.Format == MatchFormat.Group).ToList();

            foreach (var groupMatches in allGroupMatches.GroupBy(x => x.GroupName))
            {
                allMatches.Add(groupMatches.ToList());
            }

            return allMatches;
        }
    }
}
