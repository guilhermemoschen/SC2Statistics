using AutoMapper;

using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.DesktopClient.Model.Translator;
using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static IMapperConfiguration Configuration { get; private set; }

        public static void Configure()
        {
            var configuration = new MapperConfiguration(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new SC2DomainProfile());
            });

            Container.Instance.RegisterInstance<IMapper>(configuration.CreateMapper());
        }
    }
}
