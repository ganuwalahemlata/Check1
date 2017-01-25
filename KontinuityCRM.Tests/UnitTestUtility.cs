using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace KontinuityCRM.Tests
{
    public static class UnitTestUtility
    {
        public static IList<T> WithList<T>(this T item, IList<T> sourceList = null)
        {
            if (sourceList == null)
            {
                sourceList = new List<T>();
            }
            sourceList.Add(item);
            return sourceList;
        }
        public static DbSet<T> ToDbSet<T>(this IEnumerable<T> sourceList) where T : class 
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return dbSet.Object;            
        }
    }
}
