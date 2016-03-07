using Microsoft.Practices.Unity;

using SC2Statistics.SC2Domain.Repository;
using SC2Statistics.SC2Domain.Service;

namespace SC2Statistics.SC2Domain
{
    public static class UnityConfiguration
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            // Repositories
            container.RegisterType<IPlayerRespository, PlayerRespository>();
            container.RegisterType<IEventRepository, EventRepository>();
            container.RegisterType<IMatchRepository, MatchRepository>();

            // Services
            container.RegisterType<IParseService, ParseService>();
            container.RegisterType<ISC2Service, SC2Service>();
            container.RegisterType<IStatisticsService, StatisticsService>();
        }
    }
}
