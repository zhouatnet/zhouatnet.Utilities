using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace zhouatnet.Utilities
{
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int total, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            this.AddRange(items);
        }

        /// <summary>
        /// 启用/禁用 上一页
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        /// <summary>
        /// 启用/禁用 下一页
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static  PaginatedList<T> Create(
            List<T> source,int total, int pageIndex, int pageSize)
        {
            return new PaginatedList<T>(source, total, pageIndex, pageSize);
        }
    }
}
