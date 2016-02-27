using System.Windows;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;
using SC2LiquipediaStatistics.Utilities.Unity;

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

            DesktopClient.UnityConfiguration.RegisterTypes(Container.Instance);
            LiquipediaDomain.UnityConfiguration.RegisterTypes(Container.Instance);
            StatisticDomain.UnityConfiguration.RegisterTypes(Container.Instance);

            Utilities.DataBase.NHibernateAndSQLiteConfiguration.SetupDatabase(typeof(Player).Assembly, "SC2LiquipediaStatistics.db");
        }
    }
}
