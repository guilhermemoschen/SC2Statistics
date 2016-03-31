using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate;

namespace SC2Statistics.Utilities.DataBase
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        ISession Session { get; }

        void Save(TEntity entity);

        void Merge(TEntity entity);

        IList<TEntity> FindAllAndOrderBy<TKey>(Expression<Func<TEntity, TKey>> orderByExpression, int pageIndex = 0, int pageSize = 20);
        IList<TEntity> FindAllAndOrderByDescending<TKey>(Expression<Func<TEntity, TKey>> orderByExpression, int pageIndex = 0, int pageSize = 20);
        IList<TEntity> FindAll(int pageIndex, int pageSize);
        IList<TEntity> FindAll();

        TEntity Load(long id);

        void Delete(TEntity entity);
    }
}