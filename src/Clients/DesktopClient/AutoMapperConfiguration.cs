using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.DesktopClient.Model.Translator;

namespace SC2LiquipediaStatistics.DesktopClient
{
    public static class AutoMapperConfiguration
    {
        public static IMapperConfiguration Configuration { get; private set; }

        public static void Configure(IUnityContainer unityContainer)
        {
            var configuration = new MapperConfiguration(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new SC2DomainProfile());
            });

            unityContainer.RegisterInstance<IMapper>(configuration.CreateMapper());
        }
    }
}
