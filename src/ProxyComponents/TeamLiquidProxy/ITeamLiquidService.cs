using System;

using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.Proxy.TeamLiquied
{
    public interface ITeamLiquidService
    {
        Uri GetPlayerImage(Player player);
    }
}