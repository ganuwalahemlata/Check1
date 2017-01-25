using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using PagedList;

namespace KontinuityCRM.Helpers
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IContinuityDbContext Context;
        protected DbSet<TEntity> DbSet;

        public GenericRepository(IContinuityDbContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }
        /// <summary>
        /// Get Items of TEntity (Generic) Type
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        /// <summary>
        /// Get Items of TEntity (Generic) Type at specific Page
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="orderBy"></param>
        /// <param name="filter"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IPagedList<TEntity> GetPage(
           int pageSize,
            int pageNumber,
           // out int Count,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
           Expression<Func<TEntity, bool>> filter = null,

           string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            //Count = query.Count();

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            query = orderBy(query);

            return query.ToPagedList(pageNumber, pageSize);
        }
        /// <summary>
        /// Find TEntity (Generic) Type with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Find(params object[] id)
        {
            return DbSet.Find(id);
        }
        /// <summary>
        /// Add Item of TEntity (Generic) Type
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }
        /// <summary>
        /// Delete Item of TEntity (Generic) Type by Id
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }
        /// <summary>
        /// Delete Item of TEntity (Generic) Type
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null)
                return;
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }
        /// <summary>
        /// Update Item of TEntity (Generic) Type
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual void Update(TEntity entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        /// <summary>
        /// Update Item of TEntity (Generic) Type, by providing the new and old entity
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <param name="entityWithNewValues"></param>
        public virtual void Update(TEntity entityToUpdate, TEntity entityWithNewValues)
        {
            //dbSet.Attach(entityToUpdate);

            Context.Entry(entityToUpdate).CurrentValues.SetValues(entityWithNewValues);

            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        /// <summary>
        /// Get DbSet of TEntity (Generic)
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetSet()
        {
            return DbSet;
        }
    }
}