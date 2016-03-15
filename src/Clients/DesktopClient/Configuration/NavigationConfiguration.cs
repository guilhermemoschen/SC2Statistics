using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.Configuration
{
    public static class NavigationConfiguration
    {
        public static void Configure()
        {
            Container.Instance.RegisterInstance<IModernNavigationService>(CreateNavigationService());
        }

        private static IModernNavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure(ViewLocator.HomeView, new Uri("View/HomeView.xaml", UriKind.Relative));
            navigationService.Configure(ViewLocator.ListEventsView, new Uri("View/ListEventsView.xaml", UriKind.Relative));
            navigationService.Configure(ViewLocator.EditEventView, new Uri("View/EditEventView.xaml", UriKind.Relative));
            navigationService.Configure(ViewLocator.PlayerStatisticsView, new Uri("View/PlayerStatisticsView.xaml", UriKind.Relative));
            return navigationService;
        }
    }
}
