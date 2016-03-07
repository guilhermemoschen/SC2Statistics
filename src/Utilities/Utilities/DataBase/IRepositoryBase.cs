using System.Collections.Generic;

using NHibernate;

namespace SC2LiquipediaStatistics.Utilities.DataBase
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        ISession Session { get; }

        void Save(TEntity entity);

        void Merge(TEntity entity);

        IList<TEntity> FindAll();

        TEntity Load(long id);
    }
}