using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.LiquipediaDomain.Repository;
using SC2LiquipediaStatistics.LiquipediaDomain.Service;

namespace SC2LiquipediaStatistics.LiquipediaDomain
{
    public static class UnityConfiguration
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            // Repositories
            container.RegisterType<IPlayerRespository, PlayerRespository>();
            container.RegisterType<IEventRepository, EventRepository>();

            // Services
            container.RegisterType<IParseService, ParseService>();
            container.RegisterType<ILiquipediaService, LiquipediaService>();
        }
    }
}
