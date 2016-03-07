using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using GalaSoft.MvvmLight.Views;

using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;
using SC2LiquipediaStatistics.Utilities.Log;
using SC2LiquipediaStatistics.Utilities.Web;

namespace SC2LiquipediaStatistics.DesktopClient
{
    public static class UnityConfiguration
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            // ViewModels
            container.RegisterType<ListEventsViewModel>();
            container.RegisterType<AddEventViewModel>();
            container.RegisterType<PlayerStatisticsViewModel>();

            // Services
            container.RegisterType<IEventsListService, EventsListService>();

            // Navigation
            var navigationService = new NavigationService();
            navigationService.Configure(ViewLocator.Introduction, new Uri("View/IntroductionView.xaml", UriKind.Relative));
            navigationService.Configure(ViewLocator.ListEventsView, new Uri("View/ListEventsView.xaml", UriKind.Relative));
            navigationService.Configure(ViewLocator.EditEventView, new Uri("View/EditEventView.xaml", UriKind.Relative));
            navigationService.Configure(ViewLocator.PlayerStatisticsView, new Uri("View/PlayerStatisticsView.xaml", UriKind.Relative));
            container.RegisterInstance<IModernNavigationService>(navigationService);

            // Utilities
            container.RegisterType<IDownloader, Downloader>();
            container.RegisterType<ILogger, MessageLogger>();
        }
    }
}
