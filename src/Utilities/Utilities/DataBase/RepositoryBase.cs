using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NHibernate;
using NHibernate.Linq;

using SC2LiquipediaStatistics.Utilities.DataBase;

namespace SC2Statistics.Utilities.DataBase
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

        public IList<TEntity> FindAllAndOrderBy<TKey>(Expression<Func<TEntity, TKey>> orderByExpression, int pageIndex = 0, int pageSize = 20)
        {
            return Session.Query<TEntity>()
                .OrderBy(orderByExpression)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public IList<TEntity> FindAllAndOrderByDescending<TKey>(Expression<Func<TEntity, TKey>> orderByExpression, int pageIndex = 0, int pageSize = 20)
        {
            return Session.Query<TEntity>()
                .OrderByDescending(orderByExpression)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public IList<TEntity> FindAll(int pageIndex, int pageSize)
        {
            return Session.Query<TEntity>()
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public IList<TEntity> FindAll()
        {
            return Session.Query<TEntity>()
                .ToList();
        }

        public TEntity Load(long id)
        {
            return Session.Load<TEntity>(id);
        }

        public void Delete(TEntity entity)
        {
            Session.Delete(entity);
        }
    }
}
