using System.Collections.Generic;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Service
{
    public interface IStatisticsService
    {
        PlayerStatistics GeneratePlayerStatistics(long playerId, Expansion expansion);
    }
}