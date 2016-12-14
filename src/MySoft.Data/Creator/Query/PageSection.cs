using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySoft.Data
{
    /// <summary>
    /// 分页器
    /// </summary>
    public class PageSection : IPageQuery
    {
        private QuerySection<TempTable> query;
        private int? rowCount;
        private int pageSize;
        internal PageSection(QuerySection<TempTable> query, int pageSize)
        {
            this.query = query;
            this.pageSize = pageSize;
        }

        /// <summary>
        /// 返回页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (rowCount == null)
                {
                    rowCount = query.Count();
                }
                return Convert.ToInt32(Math.Ceiling(1.0 * rowCount.Value / pageSize));
            }
        }

        /// <summary>
        /// 返回记录数
        /// </summary>
        public int RowCount
        {
            get
            {
                if (rowCount == null)
                {
                    rowCount = query.Count();
                }
                return rowCount.Value;
            }
        }

        /// <summary>
        /// 返回ISourceReader
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public SourceReader ToReader(int pageIndex)
        {
            return query.GetPage(pageSize).ToReader(pageIndex);
        }

        /// <summary>
        /// 返回IArrayList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public SourceList<T> ToList<T>(int pageIndex)
            where T : Entity
        {
            return query.GetPage<T>(pageSize).ToList(pageIndex);
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public SourceTable ToTable(int pageIndex)
        {
            return query.GetPage(pageSize).ToTable(pageIndex);
        }
    }
}
