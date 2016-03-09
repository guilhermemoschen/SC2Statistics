using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
