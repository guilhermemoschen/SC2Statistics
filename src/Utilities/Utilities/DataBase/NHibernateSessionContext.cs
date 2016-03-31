using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2Statistics.Utilities.DataBase;

namespace SC2LiquipediaStatistics.Utilities.DataBase
{
    public class NHibernateSessionContext : IDisposable
    {
        public void Dispose()
        {
            NHibernateAndSQLiteConfiguration.DisposeSession();
        }
    }
}
