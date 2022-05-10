using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Tradefact.Api.Model
{
    public class PagingMetadata
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
        public bool HasPrevious { get; set; } = false;
        public bool HasNext { get; set; } = true;
    }

    public class ListResource<T>
    {
        public IPagedList<T> Items { get; set; }
        public PagingMetadata Paging { get; set; }

        public ListResource(IPagedList<T> items)
        {
            Items = items;
            this.Paging = Items != null ? new PagingMetadata
            {
                Page = Items.PageNumber,
                PageSize = Items.PageSize,
                TotalPages = Items.PageCount,
                TotalCount = Items.TotalItemCount,
                HasNext = Items.HasNextPage,
                HasPrevious = Items.HasPreviousPage
            } : new PagingMetadata();
        }

    }

}
