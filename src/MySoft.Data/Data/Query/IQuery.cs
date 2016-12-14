using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    /// <summary>
    /// ��������
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// ������
        /// </summary>
        LeftJoin,
        /// <summary>
        /// ������
        /// </summary>
        RightJoin,
        /// <summary>
        /// �ڲ�����
        /// </summary>
        InnerJoin
    }

    public interface IPaging
    {
        /// <summary>
        /// ����ǰ׺
        /// </summary>
        /// <param name="prefix"></param>
        void Prefix(string prefix);

        /// <summary>
        /// ���ú�׺
        /// </summary>
        /// <param name="suffix"></param>
        void Suffix(string suffix);

        /// <summary>
        /// ���ý�β
        /// </summary>
        /// <param name="end"></param>
        void End(string end);
    }

    interface IQuerySection<T> : IQuery<T>
        where T : Entity
    {
        #region ��������

        QuerySection<T> GroupBy(GroupByClip groupBy);
        QuerySection<T> Having(WhereClip where);
        QuerySection<T> Select(params Field[] fields);
        QuerySection<T> Select(ExcludeField field);
        QuerySection<T> Where(WhereClip where);
        QuerySection<T> Union(QuerySection<T> query);
        QuerySection<T> UnionAll(QuerySection<T> query);

        #endregion

        #region �����Ӳ�ѯ

        QuerySection<T> SubQuery();
        QuerySection<T> SubQuery(string aliasName);
        QuerySection<TSub> SubQuery<TSub>() where TSub : Entity;
        QuerySection<TSub> SubQuery<TSub>(string aliasName) where TSub : Entity;

        #endregion

        #region ��������

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
