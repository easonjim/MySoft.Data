using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 分页数据信息
    /// </summary>
    public class DataPage : IDataPage
    {
        private int pageSize;
        private int pageIndex;
        private int rowCount;

        /// <summary>
        /// 获取或设置页序数
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return pageIndex;
            }
            set
            {
                pageIndex = value;
            }
        }

        /// <summary>
        /// 获取或设置页大小
        /// </summary>
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }

        /// <summary>
        /// 获取或设置记录数
        /// </summary>
        public int RowCount
        {
            get
            {
                return rowCount;
            }
            set
            {
                rowCount = value;
            }
        }

        /// <summary>
        /// 获取或设置页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(rowCount * 1.0 / pageSize));
            }
        }

        /// <summary>
        ///  获取一个值，该值指示当前页是否是首页
        /// </summary>
        public bool IsFirstPage
        {
            get
            {
                return pageIndex <= 1;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示当前页是否是最后一页
        /// </summary>
        public bool IsLastPage
        {
            get
            {
                return pageIndex >= PageCount;
            }
        }

        /// <summary>
        /// 获取当前数据源的记录数
        /// </summary>
        public int CurrentRowCount
        {
            get
            {
                if (IsLastPage)
                {
                    return rowCount - (pageSize * (pageIndex - 1));
                }
                return pageSize;
            }
        }

        /// <summary>
        /// 获取开始记录数
        /// </summary>
        public int CurrentStartIndex
        {
            get
            {
                if (IsFirstPage)
                {
                    return 1;
                }
                return (pageIndex - 1) * pageSize + 1;
            }
        }

        /// <summary>
        /// 获取结束记录数
        /// </summary>
        public int CurrentEndIndex
        {
            get
            {
                if (IsLastPage)
                {
                    return rowCount;
                }
                return pageIndex * pageSize;
            }
        }

        private object dataSource;

        /// <summary>
        /// 获取或设置当前页的数据源
        /// </summary>
        public object DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value;
            }
        }

        /// <summary>
        /// 初始化DataPage
        /// </summary>
        public DataPage()
        {
            this.pageIndex = 1;
        }

        /// <summary>
        /// 设置默认页大小
        /// </summary>
        /// <param name="pageSize"></param>
        public DataPage(int pageSize)
            : this()
        {
            this.pageSize = pageSize;
        }
    }

    /// <summary>
    /// 分页数据信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataPage<TSource> : DataPage, IDataPage<TSource>
    {
        /// <summary>
        /// 获取或设置当前页的数据源
        /// </summary>
        public new TSource DataSource
        {
            get
            {
                return (TSource)base.DataSource;
            }
            set
            {
                base.DataSource = value;
            }
        }

        /// <summary>
        /// 初始化DataPage
        /// </summary>
        public DataPage()
            : base()
        { }

        /// <summary>
        /// 设置默认页大小
        /// </summary>
        /// <param name="pageSize"></param>
        public DataPage(int pageSize)
            : base(pageSize)
        { }
    }
}
