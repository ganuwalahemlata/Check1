using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KontinuityCRM.Helpers.GRepository
{
    public interface IIdentifiable
    {
        int Id { get; }
    }

    public interface IMyContext : IDisposable
    {
        int SaveChanges();
    }
    
    public interface IUnitOfWork : IDisposable
    {
        int Save();
        IMyContext Context { get; }
    }

    public interface IRepository<T> : IDisposable where T : class, IIdentifiable
    {
        IQueryable<T> All { get; }
        IQueryable<T> AllEager(params Expression<Func<T, object>>[] includes);
        T Find(int id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(int id);
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMyContext context;
        public UnitOfWork()
        {
            context = null; // new KontinuityDB();
        }
        public UnitOfWork(IMyContext context)
        {
            this.context = context;
        }
        public int Save()
        {
            return context.SaveChanges();
        }
        public IMyContext Context
        {

            get
            {
                return context;
            }
        }
        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }

    public class Repository<T> : IRepository<T> where T : class, IIdentifiable
    {
        private readonly KontinuityDB context;
        public Repository(IUnitOfWork uow)
        {
            context = uow.Context as KontinuityDB;
        }
        public IQueryable<T> All
        {
            get
            {
                return context.Set<T>();
            }
        }
        public IQueryable<T> AllEager(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }
        public T Find(int id)
        {
            return context.Set<T>().Find(id);
        }
        public void Insert(T item)
        {
            context.Entry(item).State = EntityState.Added;
        }
        public void Update(T item)
        {
            context.Set<T>().Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }
        public void Delete(int id)
        {
            var item = context.Set<T>().Find(id);
            context.Set<T>().Remove(item);
        }
        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }
    }

}
