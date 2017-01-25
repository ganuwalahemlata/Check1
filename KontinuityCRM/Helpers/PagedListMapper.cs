using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace KontinuityCRM.Helpers
{
    public class PagedListMapper<T> : IPagedList<T>
    {
        protected readonly List<T> Subset = new List<T>();

        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
        public int FirstItemOnPage { get; private set; }
        public int LastItemOnPage { get; private set; }

        public PagedListMapper(IPagedList pagelist, IEnumerable<T> items)
        {
            PageCount = pagelist.PageCount;
            TotalItemCount = pagelist.TotalItemCount;
            PageNumber = pagelist.PageNumber;
            PageSize = pagelist.PageSize;
            HasPreviousPage = pagelist.HasPreviousPage;
            HasNextPage = pagelist.HasNextPage;
            IsFirstPage = pagelist.IsFirstPage;
            IsLastPage = pagelist.IsLastPage;
            FirstItemOnPage = pagelist.FirstItemOnPage;
            LastItemOnPage = pagelist.LastItemOnPage;

            Subset = items.ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Subset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IPagedList GetMetaData()
        {
            return new PagedListMetaData(this);
        }

        public T this[int index]
        {
            get { return Subset[index]; }
        }

        public int Count
        {
            get { return Subset.Count; }
        }
    }
}