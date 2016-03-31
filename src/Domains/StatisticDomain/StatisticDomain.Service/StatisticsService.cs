using System;
using System.Linq;

using SC2Statistics.StatisticDomain.Database.Repository;
using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Service
{
    public class StatisticsService : IStatisticsService
    {
        public IMatchRepository MatchRepository { get; protected set; }
        public IPlayerRespository PlayerRespository { get; protected set; }

        public StatisticsService(IMatchRepository matchRepository, IPlayerRespository playerRespository)
        {
            MatchRepository = matchRepository;
            PlayerRespository = playerRespository;
        }

        public PlayerStatistics GeneratePlayerStatistics(long playerId, Expansion expansion)
        {
            var player = PlayerRespository.Load(playerId);

            if (player == null)
                throw new ArgumentNullException(nameof(playerId));

            var allMatches = MatchRepository.FindMatchesByPlayerAndExpansion(player.Id, expansion);

            if (!allMatches.Any())
                return null;

            var matchesWon = allMatches.Where(x => x.Winner.Id == player.Id).ToList();

            var statistics = new PlayerStatistics()
            {
                Player = player,
                WinRate = (decimal)matchesWon.Count / (decimal)allMatches.Count,
                TotalWins = matchesWon.Count,
                TotalMatches = allMatches.Count,
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
                statistics.TotalMatchesAgainstZerg = matchesXZerg.Count;
                statistics.TotalWinsAgainstZerg = matchesWonXZerg.Count;
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
                statistics.TotalMatchesAgainstProtoss = matchesXProtoss.Count;
                statistics.TotalWinsAgainstProtoss = matchesWonXProtoss.Count;
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
                statistics.TotalMatchesAgainstTerran = matchesXTerran.Count;
                statistics.TotalWinsAgainstTerran = matchesWonXTerran.Count;
            }

            var matchesXKoreans = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2.Country == "KR") ||
                    (x.Player2.Id == player.Id && x.Player1.Country == "KR")
                )
                .ToList();

            if (matchesXKoreans.Any())
            {
                var matchesWonXKoreans = matchesXKoreans.Where(x => x.Winner.Id == player.Id).ToList();
                statistics.WinRateXKoreans = (decimal)matchesWonXKoreans.Count / (decimal)matchesXKoreans.Count;
                statistics.TotalMatchesAgainstKoreans = matchesXKoreans.Count;
                statistics.TotalWinsAgainstKoreans = matchesWonXKoreans.Count;
            }

            var matchesXForeigners = allMatches
                .Where(x =>
                    (x.Player1.Id == player.Id && x.Player2.Country != "KR") ||
                    (x.Player2.Id == player.Id && x.Player1.Country != "KR")
                )
                .ToList();

            if (matchesXForeigners.Any())
            {
                var matchesWonXForeigners = matchesXForeigners.Where(x => x.Winner.Id == player.Id).ToList();
                statistics.WinRateXForeigners = (decimal)matchesWonXForeigners.Count / (decimal)matchesXForeigners.Count;
                statistics.TotalMatchesAgainstForeigners = matchesXForeigners.Count;
                statistics.TotalWinsAgainstForeigners = matchesWonXForeigners.Count;
            }

            return statistics;
        }
    }
}
