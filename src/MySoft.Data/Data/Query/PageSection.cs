using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    public class PageSection<T> : IPageSection<T>
        where T : Entity
    {
        private QuerySection<T> query;
        private int? rowCount;
        private int pageSize;
        internal PageSection(QuerySection<T> query, int pageSize)
        {
            this.pageSize = pageSize;
            this.query = query;
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

        #region 返回object

        /// <summary>
        /// 返回一个Object列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ArrayList<object> ToListResult(int pageIndex)
        {
            int startIndex = pageSize * (pageIndex - 1) + 1;
            int endIndex = pageSize * pageIndex;
            return query.ToListResult(startIndex, endIndex);
        }

        /// <summary>
        /// 返回一个Object列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ArrayList<TResult> ToListResult<TResult>(int pageIndex)
        {
            int startIndex = pageSize * (pageIndex - 1) + 1;
            int endIndex = pageSize * pageIndex;
            return query.ToListResult<TResult>(startIndex, endIndex);
        }

        #endregion

        /// <summary>
        /// 返回一个实体
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public T ToSingle(int pageIndex)
        {
            int startIndex = pageSize * (pageIndex - 1) + 1;
            int endIndex = pageSize * pageIndex;
            IList<T> list = query.ToList(startIndex, endIndex);
            if (list.Count == 0)
            {
                return default(T);
            }
            else
            {
                return list[0];
            }
        }

        /// <summary>
        /// 返回一个DbReader
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public SourceReader ToReader(int pageIndex)
        {
            int startIndex = pageSize * (pageIndex - 1) + 1;
            int endIndex = pageSize * pageIndex;
            return query.ToReader(startIndex, endIndex);
        }

        /// <summary>
        /// 返回一个DataTable
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public SourceTable ToTable(int pageIndex)
        {
            int startIndex = pageSize * (pageIndex - 1) + 1;
            int endIndex = pageSize * pageIndex;
            return query.ToTable(startIndex, endIndex);
        }

        /// <summary>
        /// 返回一个IArrayList
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public SourceList<T> ToList(int pageIndex)
        {
            int startIndex = pageSize * (pageIndex - 1) + 1;
            int endIndex = pageSize * pageIndex;
            return query.ToList(startIndex, endIndex);
        }
    }
}
