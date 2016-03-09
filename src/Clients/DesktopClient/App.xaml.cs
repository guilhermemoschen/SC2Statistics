using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using FirstFloor.ModernUI.Windows.Controls;

using NHibernate.Linq;

using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Container.Configure();

            AutoMapperConfiguration.Configure(Container.Instance);

            DesktopClient.UnityConfiguration.RegisterTypes(Container.Instance);
            SC2Statistics.SC2Domain.UnityConfiguration.RegisterTypes(Container.Instance);

            Utilities.DataBase.NHibernateAndSQLiteConfiguration.SetupDatabase(typeof(Player).Assembly, ConfigurationManager.AppSettings["DataBaseFilePath"]);

            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            ModernDialog.ShowMessage("Something wrong happend :o", "Oh, no!", MessageBoxButton.OK);
            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }
    }
}
