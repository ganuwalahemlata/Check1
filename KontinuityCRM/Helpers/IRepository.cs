using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PagedList;

namespace KontinuityCRM.Helpers
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        T Find(params object[] id);
        void Add(T entity);
        void Update(T entity);
        void Delete(object id);
        void Delete(T entity);
        void Update(T entityToUpdate, T entityWithNewValues);        
        IQueryable<T> GetSet();

        IPagedList<T> GetPage(int pageSize,
            int pageNumber,
            //out int Count, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            Expression<Func<T, bool>> filter = null, 
            string includeProperties = "");
    }
}