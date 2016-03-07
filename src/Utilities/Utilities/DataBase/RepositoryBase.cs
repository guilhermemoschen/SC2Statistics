using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate;
using NHibernate.Linq;

namespace SC2LiquipediaStatistics.Utilities.DataBase
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        public ISession Session
        {
            get { return NHibernateAndSQLiteConfiguration.CurrentSession; }
        }

        public void Save(TEntity entity)
        {
            Session.Save(entity);
        }

        public void Merge(TEntity entity)
        {
            Session.Merge(entity);
        }

        public IList<TEntity> FindAll()
        {
            return Session.Query<TEntity>().ToList();
        }

        public TEntity Load(long id)
        {
            return Session.Load<TEntity>(id);
        }
    }
}
