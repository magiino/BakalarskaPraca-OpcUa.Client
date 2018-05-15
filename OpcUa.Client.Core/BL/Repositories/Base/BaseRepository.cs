using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace OpcUa.Client.Core
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
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

        public IEnumerable<TEntity> GetAll()
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
            var addedEntity = DbSet.Add(entity);
            Context.SaveChanges();
            return addedEntity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            var addedEntities = DbSet.AddRange(entities);
            Context.SaveChanges();
            return addedEntities;
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
            Context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
            Context.SaveChanges();
        }
    }
}