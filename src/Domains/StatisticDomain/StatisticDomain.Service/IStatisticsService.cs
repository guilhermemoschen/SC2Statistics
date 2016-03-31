using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Service
{
    public interface IStatisticsService
    {
        PlayerStatistics GeneratePlayerStatistics(long playerId, Expansion expansion);
    }
}