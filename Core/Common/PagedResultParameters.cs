using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common
{
	public class PagedResultParameters
	{
		const int maxPageSize = 50;
		public virtual bool NoPage { get; set; } = false;

		private int _pageNumber = 1;
		private int _pageSize = 10;

		public int PageNumber
		{
			get
			{
				return (NoPage) ? 1 : _pageNumber;
			}
			set
			{
				_pageNumber = value;
			}
		}

		public int PageSize
		{
			get
			{
				return (NoPage) ? int.MaxValue : _pageSize;
			}
			set
			{
				_pageSize = (value > maxPageSize) ? maxPageSize : value;
			}
		}
	}
}
