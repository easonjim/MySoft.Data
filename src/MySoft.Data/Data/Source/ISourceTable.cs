using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 数据源接口
    /// </summary>
    interface ISourceTable : IListConvert<IRowReader>, IDisposable
    {
        #region 常用方法

        /// <summary>
        /// 获取数据行数
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// 获取数据列数
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// 获取行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IRowReader this[int index] { get; }

        /// <summary>
        /// 克隆Table
        /// </summary>
        /// <returns></returns>
        SourceTable Clone();

        /// <summary>
        /// 选择某些列
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        SourceTable Select(params string[] names);

        /// <summary>
        /// 过虑数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        SourceTable Filter(string expression);

        /// <summary>
        /// 排序数据
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        SourceTable Sort(string sort);

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        void Add(string name, Type type);

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        void Add(string name, Type type, string expression);

        /// <summary>
        /// 设置列的顺序
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        void SetOrdinal(string name, int index);

        /// <summary>
        /// 字段更名
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        /// <returns></returns>
        void Rename(string oldname, string newname);

        /// <summary>
        /// 按要求改变某列值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="readName"></param>
        /// <param name="changeName"></param>
        /// <param name="revalue"></param>
        void Revalue<T>(string readName, string changeName, ReturnValue<T> revalue);

        /// <summary>
        /// 移除指定的列
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        void Remove(params string[] names);

        /// <summary>
        /// 将另一个表中的某字段值按字段关联后进行填充
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        void Fill(FillRelation relation, params string[] fillNames);

        #endregion
    }
}