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
        /// ����ҳ��
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
        /// ���ؼ�¼��
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

        #region ����object

        /// <summary>
        /// ����һ��Object�б�
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
        /// ����һ��Object�б�
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
        /// ����һ��ʵ��
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
        /// ����һ��DbReader
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
        /// ����һ��DataTable
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
        /// ����һ��IArrayList
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
