using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 
    /// </summary>
    interface IDataPage
    {
        /// <summary>
        /// 获取或设置页大小
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 获取或设置记录数
        /// </summary>
        int RowCount { get; set; }
        /// <summary>
        /// 获取或设置页序数
        /// </summary>
        int CurrentPageIndex { get; set; }
        /// <summary>
        /// 获取或设置页数
        /// </summary>
        int PageCount { get; }
        /// <summary>
        ///  获取一个值，该值指示当前页是否是首页
        /// </summary>
        bool IsFirstPage { get; }
        /// <summary>
        /// 获取一个值，该值指示当前页是否是最后一页
        /// </summary>
        bool IsLastPage { get; }
        /// <summary>
        /// 获取当前数据源的记录数
        /// </summary>
        int CurrentRowCount { get; }
        /// <summary>
        /// 获取开始记录数
        /// </summary>
        int CurrentStartIndex { get; }
        /// <summary>
        /// 获取结束记录数
        /// </summary>
        int CurrentEndIndex { get; }

        /// <summary>
        /// 获取或设置当前页的数据源
        /// </summary>
        object DataSource { get; set; }
    }

    interface IDataPage<T> : IDataPage
    {
        /// <summary>
        /// 获取或设置当前页的数据源
        /// </summary>
        new T DataSource { get; set; }
    }
}
