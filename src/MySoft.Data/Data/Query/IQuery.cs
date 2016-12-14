using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    /// <summary>
    /// 连接类型
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// 左链接
        /// </summary>
        LeftJoin,
        /// <summary>
        /// 右链接
        /// </summary>
        RightJoin,
        /// <summary>
        /// 内部链接
        /// </summary>
        InnerJoin
    }

    public interface IPaging
    {
        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="prefix"></param>
        void Prefix(string prefix);

        /// <summary>
        /// 设置后缀
        /// </summary>
        /// <param name="suffix"></param>
        void Suffix(string suffix);

        /// <summary>
        /// 设置结尾
        /// </summary>
        /// <param name="end"></param>
        void End(string end);
    }

    interface IQuerySection<T> : IQuery<T>
        where T : Entity
    {
        #region 分组排序

        QuerySection<T> GroupBy(GroupByClip groupBy);
        QuerySection<T> Having(WhereClip where);
        QuerySection<T> Select(params Field[] fields);
        QuerySection<T> Select(ExcludeField field);
        QuerySection<T> Where(WhereClip where);
        QuerySection<T> Union(QuerySection<T> query);
        QuerySection<T> UnionAll(QuerySection<T> query);

        #endregion

        #region 创建子查询

        QuerySection<T> SubQuery();
        QuerySection<T> SubQuery(string aliasName);
        QuerySection<TSub> SubQuery<TSub>() where TSub : Entity;
        QuerySection<TSub> SubQuery<TSub>(string aliasName) where TSub : Entity;

        #endregion

        #region 返回数据

        ArrayList<object> ToListResult();
        ArrayList<object> ToListResult(int startIndex, int endIndex);

        ArrayList<TResult> ToListResult<TResult>();
        ArrayList<TResult> ToListResult<TResult>(int startIndex, int endIndex);

        SourceReader ToReader();
        SourceReader ToReader(int startIndex, int endIndex);

        TResult ToScalar<TResult>();
        object ToScalar();
        int Count();

        #endregion

        DataPage<IList<T>> ToListPage(int pageSize, int pageIndex);
        DataPage<DataTable> ToTablePage(int pageSize, int pageIndex);
    }

    interface IQuery<T>
        where T : Entity
    {
        T ToSingle();

        SourceTable ToTable();
        SourceTable ToTable(int startIndex, int endIndex);

        SourceList<T> ToList();
        SourceList<T> ToList(int startIndex, int endIndex);

        QuerySection<T> SetPagingField(Field pagingField);
        QuerySection<T> Distinct();
        QuerySection<T> OrderBy(OrderByClip orderBy);
        TopSection<T> GetTop(int topSize);
        PageSection<T> GetPage(int pageSize);

        PageSection<TEntity> GetPage<TEntity>(int pageSize) where TEntity : Entity;

        TEntity ToSingle<TEntity>() where TEntity : Entity;

        SourceList<TEntity> ToList<TEntity>() where TEntity : Entity;
        SourceList<TEntity> ToList<TEntity>(int startIndex, int endIndex) where TEntity : Entity;
    }
}
