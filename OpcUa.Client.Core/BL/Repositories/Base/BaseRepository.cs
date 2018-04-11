using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace OpcUa.Client.Core
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly DbContext Context;

        private DbSet<TEntity> DbSet => Context.Set<TEntity>();

        public BaseRepository(DbContext context)
        {
            Context = context;
        }

        public TEntity GetById(int id)
        {
            return DbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAllWithEndpoints()
        {
            return DbSet.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.SingleOrDefault(predicate);
        }

        public TEntity Add(TEntity entity)
        {
            return DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}