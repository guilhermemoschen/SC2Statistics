using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2Statistics.StatisticDomain.Database.Mapping;
using SC2Statistics.Utilities.DataBase;

namespace SC2LiquipediaStatistics.DesktopClient.Configuration
{
    public static class DataBaseConfiguration
    {
        public static void Configure()
        {
            var dbFilePath = GetDatabaseFilePath();
            NHibernateAndSQLiteConfiguration.SetupDatabase(typeof(PlayerMap).Assembly, dbFilePath);
        }

        private static string GetDatabaseFilePath()
        {
#if DEBUG
            var dataBaseFilePath = @"..\..\..\..\";
#else
            var dataBaseFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dataBaseFilePath = Path.Combine(dataBaseFilePath, "SC2Statistics");
#endif
            return Path.Combine(dataBaseFilePath, ConfigurationManager.AppSettings["DataBaseFilePath"]);
        }
    }
}
