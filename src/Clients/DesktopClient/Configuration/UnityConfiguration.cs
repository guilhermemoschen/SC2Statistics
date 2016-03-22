using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;
using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Unity;
using SC2LiquipediaStatistics.Utilities.Web;

namespace SC2LiquipediaStatistics.DesktopClient.Configuration
{
    public static class UnityConfiguration
    {
        public static void Configure()
        {
            Container.Configure();

            // ViewModels
            Container.Instance.RegisterType<ListEventsViewModel>();
            Container.Instance.RegisterType<AddEventViewModel>();
            Container.Instance.RegisterType<EditEventViewModel>();
            Container.Instance.RegisterType<PlayerStatisticsViewModel>();
            Container.Instance.RegisterType<MainViewModel>();
            Container.Instance.RegisterType<LoadingViewModel>();
            Container.Instance.RegisterType<PlayerByEventStatisticsViewModel>();

            // Services
            Container.Instance.RegisterType<IEventsListService, EventsListService>();

            // Loading
            Container.Instance.RegisterType<ILoadingService, LoadingService>();

            // Utilities
            Container.Instance.RegisterType<IDownloader, Downloader>();
            Container.Instance.RegisterType<ILogger, MessageLogger>();

            // SC2Domain
            SC2Statistics.SC2Domain.UnityConfiguration.RegisterTypes(Container.Instance);
        }
    }
}
