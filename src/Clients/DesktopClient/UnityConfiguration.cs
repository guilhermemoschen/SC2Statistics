using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Service;
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
            container.RegisterType<MenuViewModel>();
            container.RegisterType<EventViewModel>();
            container.RegisterType<UpdateDataBaseViewModel>();

            // Services
            container.RegisterType<IEventsListService, EventsListService>();

            // Utilities
            container.RegisterType<IDownloader, Downloader>();
            container.RegisterType<ILogger, MessageLogger>();
        }
    }
}
