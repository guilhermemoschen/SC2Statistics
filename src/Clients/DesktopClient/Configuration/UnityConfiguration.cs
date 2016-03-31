﻿using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;
using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Unity;
using SC2LiquipediaStatistics.Utilities.Web;

using SC2Statistics.Proxy.Aligulac;
using SC2Statistics.SC2Domain.Repository;
using SC2Statistics.SC2Domain.Service;

namespace SC2LiquipediaStatistics.DesktopClient.Configuration
{
    public static class UnityConfiguration
    {
        public static void Configure()
        {
            Container.Configure();

            // ViewModels
            Container.Instance.RegisterType<ListEventsViewModel>();
            Container.Instance.RegisterType<ListPlayersViewModel>();
            Container.Instance.RegisterType<EditEventViewModel>();
            Container.Instance.RegisterType<PlayerStatisticsViewModel>();
            Container.Instance.RegisterType<MainViewModel>();
            Container.Instance.RegisterType<LoadingViewModel>();

            // Services
            Container.Instance.RegisterType<IEventsListService, EventsListService>();

            // Loading
            Container.Instance.RegisterType<ILoadingService, LoadingService>();

            // Utilities
            Container.Instance.RegisterType<IDownloader, Downloader>();
            Container.Instance.RegisterType<ILogger, MessageLogger>();

            ConfigureSC2Domain(Container.Instance);
            ConfigureAligulacProxy(Container.Instance);
        }

        public static void ConfigureSC2Domain(IUnityContainer container)
        {
            // Repositories
            container.RegisterType<IPlayerRespository, PlayerRespository>();
            container.RegisterType<IEventRepository, EventRepository>();
            container.RegisterType<IMatchRepository, MatchRepository>();

            // Services
            container.RegisterType<ISC2Service, SC2Service>();
            container.RegisterType<IStatisticsService, StatisticsService>();
        }

        public static void ConfigureAligulacProxy(IUnityContainer container)
        {
            // Services
            container.RegisterType<IAligulacService, AligulacService>();
        }
    }
}
