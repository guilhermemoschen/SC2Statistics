using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate;

namespace SC2LiquipediaStatistics.Utilities.DataBase
{
    public class RepositoryBase<TEntity> where TEntity : class
    {
        protected ISession Session
        {
            get { return NHibernateAndSQLiteConfiguration.CurrentSession; }
        }

        public void Save(TEntity entity)
        {
            Session.Save(entity);
        }
    }
}
