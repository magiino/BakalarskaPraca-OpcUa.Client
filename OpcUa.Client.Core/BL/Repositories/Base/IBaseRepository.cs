using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpcUa.Client.Core
{
    // TODO ako spravit aby som s jendou instanciou pristupoval k vsetkym entitam
    public interface IBaseRepository<TEntity> where TEntity : class, IEntity
    {
        TEntity GetById(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}