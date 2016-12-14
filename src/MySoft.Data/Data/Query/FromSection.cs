using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using MySoft.Data.Design;

namespace MySoft.Data
{
    public class FromSection<T> : IQuerySection<T>
        where T : Entity
    {
        private Entity fromEntity;
        private Table fromTable;
        private QuerySection<T> query;
        internal QuerySection<T> Query
        {
            get { return query; }
        }
        private List<Entity> entityList = new List<Entity>();
        internal List<Entity> EntityList
        {
            get { return entityList; }
            set { entityList = value; }
        }

        #region 初始化FromSection

        internal FromSection()
        {
            this.fromEntity = DataUtils.CreateInstance<T>();
            this.fromTable = this.fromEntity.GetTable();
        }

        internal FromSection(DbProvider dbProvider, DbTrans dbTran, Table table)
            : this()
        {
            this.entityList.Add(this.fromEntity);
            //从传入的表信息中获取表名
            this.tableName = table == null ? fromTable.Name : table.Name;
            fromTable.As(this.tableName);
            Field pagingField = fromEntity.PagingField;
            this.query = new QuerySection<T>(this, dbProvider, dbTran, pagingField);
        }

        internal FromSection(DbProvider dbProvider, DbTrans dbTran, string aliasName)
            : this()
        {
            fromTable.As(aliasName);
            this.entityList.Add(this.fromEntity);
            this.tableName = fromTable.Name;
            Field pagingField = fromEntity.PagingField;
            if (aliasName != null)
            {
                if ((IField)pagingField != null)
                {
                    pagingField = pagingField.At(aliasName);
                }

                this.tableName += " {0}" + aliasName + "{1}";
            }
            this.query = new QuerySection<T>(this, dbProvider, dbTran, pagingField);
        }

        internal FromSection(string tableName, string relation, IList<Entity> list)
        {
            this.tableName = tableName;
            this.relation = relation;
            this.entityList.AddRange(list);
            this.query = new QuerySection<T>(this);
        }

        internal FromSection(string tableName, string aliasName)
            : this()
        {
            fromTable.As(aliasName);
            this.entityList.Add(this.fromEntity);
            this.tableName = tableName;
            if (aliasName != null)
            {
                this.tableName += " {0}" + aliasName + "{1}";
            }
            this.query = new QuerySection<T>(this);
        }

        internal FromSection(Table table)
            : this()
        {
            this.tableName = table == null ? fromTable.Name : table.Name;
            fromTable.As(this.tableName);
            this.query = new QuerySection<T>(this);
        }

        #endregion

        #region 实现IQuerySection

        #region 实现IDataQuery

        /// <summary>
        /// 返回一个分页处理的Page节
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageSection<TEntity> GetPage<TEntity>(int pageSize)
            where TEntity : Entity
        {
            return query.GetPage<TEntity>(pageSize);
        }

        /// <summary>
        /// 返回一个实体
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TEntity ToSingle<TEntity>()
            where TEntity : Entity
        {
            return query.ToSingle<TEntity>();
        }

        #endregion

        #region 创建子查询

        /// <summary>
        /// 生成一个子查询
        /// </summary>
        /// <returns></returns>
        public QuerySection<T> SubQuery()
        {
            return query.SubQuery();
        }

        /// <summary>
        /// 生成一个子查询
        /// </summary>
        /// <returns></returns>
        public QuerySection<T> SubQuery(string aliasName)
        {
            return query.SubQuery(aliasName);
        }

        /// <summary>
        /// 生成一个子查询
        /// </summary>
        /// <returns></returns>
        public QuerySection<TSub> SubQuery<TSub>()
            where TSub : Entity
        {
            return query.SubQuery<TSub>();
        }

        /// <summary>
        /// 生成一个带别名的子查询
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public QuerySection<TSub> SubQuery<TSub>(string aliasName)
            where TSub : Entity
        {
            return query.SubQuery<TSub>(aliasName);
        }

        #endregion

        //设置查询节
        internal void SetQuerySection(QuerySection<T> query)
        {
            this.query = query;
        }

        #region 排序分组操作

        /// <summary>
        /// 进行GroupBy操作
        /// </summary>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        public QuerySection<T> GroupBy(GroupByClip groupBy)
        {
            return query.GroupBy(groupBy);
        }

        /// <summary>
        /// 进行OrderBy操作
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public QuerySection<T> OrderBy(OrderByClip orderBy)
        {
            return query.OrderBy(orderBy);
        }

        /// <summary>
        /// 选择输出的列
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public QuerySection<T> Select(params Field[] fields)
        {
            return query.Select(fields);
        }

        /// <summary>
        /// 选择被排除以外的列（用于列多时排除某几列的情况）
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public QuerySection<T> Select(ExcludeField field)
        {
            return query.Select(field);
        }

        /// <summary>
        /// 注入当前查询的条件
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public QuerySection<T> Where(WhereClip where)
        {
            return query.Where(where);
        }

        /// <summary>
        /// 进行Union操作
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public QuerySection<T> Union(QuerySection<T> uquery)
        {
            return query.Union(uquery);
        }

        /// <summary>
        /// 进行Union操作
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public QuerySection<T> UnionAll(QuerySection<T> uquery)
        {
            return query.UnionAll(uquery);
        }

        /// <summary>
        /// 进行Having操作
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public QuerySection<T> Having(WhereClip where)
        {
            return query.Having(where);
        }

        /// <summary>
        /// 设置分页字段
        /// </summary>
        /// <param name="pagingField"></param>
        /// <returns></returns>
        public QuerySection<T> SetPagingField(Field pagingField)
        {
            return query.SetPagingField(pagingField);
        }

        /// <summary>
        /// 选择前n条
        /// </summary>
        /// <param name="topSize"></param>
        /// <returns></returns>
        public TopSection<T> GetTop(int topSize)
        {
            return query.GetTop(topSize);
        }

        /// <summary>
        /// 进行Distinct操作
        /// </summary>
        /// <returns></returns>
        public QuerySection<T> Distinct()
        {
            return query.Distinct();
        }

        #endregion

        #region 返回数据

        /// <summary>
        /// 返回一个分页处理的Page节
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageSection<T> GetPage(int pageSize)
        {
            return query.GetPage(pageSize);
        }

        /// <summary>
        /// 返回一个实体
        /// </summary>
        /// <returns></returns>
        public T ToSingle()
        {
            return query.ToSingle();
        }

        #region 返回object

        /// <summary>
        /// 返回一个Object列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public ArrayList<object> ToListResult(int startIndex, int endIndex)
        {
            return query.ToListResult(startIndex, endIndex);
        }

        /// <summary>
        /// 返回一个Object列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public ArrayList<object> ToListResult()
        {
            return query.ToListResult();
        }

        /// <summary>
        /// 返回一个Object列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public ArrayList<TResult> ToListResult<TResult>(int startIndex, int endIndex)
        {
            return query.ToListResult<TResult>(startIndex, endIndex);
        }

        /// <summary>
        /// 返回一个Object列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public ArrayList<TResult> ToListResult<TResult>()
        {
            return query.ToListResult<TResult>();
        }

        #endregion

        /// <summary>
        /// 返回一个DbReader
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public SourceReader ToReader(int startIndex, int endIndex)
        {
            return query.ToReader(startIndex, endIndex);
        }

        /// <summary>
        /// 返回一个DbReader
        /// </summary>
        /// <returns></returns>
        public SourceReader ToReader()
        {
            return query.ToReader();
        }

        #region 数据查询

        /// <summary>
        /// 返回IArrayList
        /// </summary>
        /// <returns></returns>
        public SourceList<T> ToList()
        {
            return query.ToList();
        }

        /// <summary>
        /// 返回IArrayList
        /// </summary>
        /// <returns></returns>
        public SourceList<T> ToList(int startIndex, int endIndex)
        {
            return query.ToList(startIndex, endIndex);
        }

        /// <summary>
        /// 返回IArrayList
        /// </summary>
        /// <returns></returns>
        public SourceList<TEntity> ToList<TEntity>() where TEntity : Entity
        {
            return query.ToList<TEntity>();
        }

        /// <summary>
        /// 返回IArrayList
        /// </summary>
        /// <returns></returns>
        public SourceList<TEntity> ToList<TEntity>(int startIndex, int endIndex) where TEntity : Entity
        {
            return query.ToList<TEntity>(startIndex, endIndex);
        }

        #endregion

        /// <summary>
        /// 返回一个DataTable
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public SourceTable ToTable(int startIndex, int endIndex)
        {
            return query.ToTable(startIndex, endIndex);
        }

        /// <summary>
        /// 返回一个DataTable
        /// </summary>
        /// <returns></returns>
        public SourceTable ToTable()
        {
            return query.ToTable();
        }

        /// <summary>
        /// 返回一个值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult ToScalar<TResult>()
        {
            return query.ToScalar<TResult>();
        }

        /// <summary>
        /// 返回一个值
        /// </summary>
        /// <returns></returns>
        public object ToScalar()
        {
            return query.ToScalar();
        }

        /// <summary>
        /// 返回当前查询记录数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return query.Count();
        }

        #endregion

        #region 返回分页信息

        public DataPage<IList<T>> ToListPage(int pageSize, int pageIndex)
        {
            return query.ToListPage(pageSize, pageIndex);
        }

        /// <summary>
        /// 返回指定数据源的分页信息
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public DataPage<DataTable> ToTablePage(int pageSize, int pageIndex)
        {
            return query.ToTablePage(pageSize, pageIndex);
        }

        #endregion

        #endregion

        #region 连接查询

        #region 内连接

        public FromSection<T> InnerJoin<TJoin>(WhereClip onWhere)
            where TJoin : Entity
        {
            return InnerJoin<TJoin>((Table)null, onWhere);
        }

        public FromSection<T> InnerJoin<TJoin>(Table table, WhereClip onWhere)
            where TJoin : Entity
        {
            return Join<TJoin>(table, onWhere, JoinType.InnerJoin);
        }

        public FromSection<T> InnerJoin<TJoin>(string aliasName, WhereClip onWhere)
            where TJoin : Entity
        {
            return Join<TJoin>(aliasName, onWhere, JoinType.InnerJoin);
        }

        #endregion

        #region 左连接

        public FromSection<T> LeftJoin<TJoin>(WhereClip onWhere)
            where TJoin : Entity
        {
            return LeftJoin<TJoin>((Table)null, onWhere);
        }

        public FromSection<T> LeftJoin<TJoin>(Table table, WhereClip onWhere)
            where TJoin : Entity
        {
            return Join<TJoin>(table, onWhere, JoinType.LeftJoin);
        }

        public FromSection<T> LeftJoin<TJoin>(string aliasName, WhereClip onWhere)
            where TJoin : Entity
        {
            return Join<TJoin>(aliasName, onWhere, JoinType.LeftJoin);
        }

        #endregion

        #region 右连接

        public FromSection<T> RightJoin<TJoin>(WhereClip onWhere)
            where TJoin : Entity
        {
            return RightJoin<TJoin>((Table)null, onWhere);
        }

        public FromSection<T> RightJoin<TJoin>(Table table, WhereClip onWhere)
            where TJoin : Entity
        {
            return Join<TJoin>(table, onWhere, JoinType.RightJoin);
        }

        public FromSection<T> RightJoin<TJoin>(string aliasName, WhereClip onWhere)
            where TJoin : Entity
        {
            return Join<TJoin>(aliasName, onWhere, JoinType.RightJoin);
        }

        #endregion

        #region 私有方法

        private FromSection<T> Join<TJoin>(Table table, WhereClip onWhere, JoinType joinType)
            where TJoin : Entity
        {
            TJoin entity = DataUtils.CreateInstance<TJoin>();
            this.entityList.Add(entity);

            if ((IField)query.PagingField == null)
            {
                //标识列和主键优先,包含ID的列被抛弃
                query.PagingField = entity.PagingField;
            }

            FromSection<TJoin> from = new FromSection<TJoin>(table);
            string strJoin = string.Empty;
            if (onWhere != null)
            {
                strJoin = " on " + onWhere.ToString();
            }

            string join = GetJoinEnumString(joinType);

            if (this.relation != null)
            {
                this.tableName = " {2} " + this.tableName;
                this.relation += " {3} ";
            }
            this.relation += join + from.TableName + strJoin;

            return this;
        }

        private FromSection<T> Join<TJoin>(string aliasName, WhereClip onWhere, JoinType joinType)
            where TJoin : Entity
        {

            TJoin entity = DataUtils.CreateInstance<TJoin>();
            entity.GetTable().As(aliasName);
            this.entityList.Add(entity);

            if ((IField)query.PagingField == null)
            {
                //标识列和主键优先,包含ID的列被抛弃
                query.PagingField = entity.PagingField;
            }

            FromSection<TJoin> from = new FromSection<TJoin>(null);
            if (aliasName != null)
            {
                if ((IField)query.PagingField != null)
                    query.PagingField = query.PagingField.At(aliasName);

                from.tableName += " {0}" + aliasName + "{1}";
            }
            string strJoin = string.Empty;
            if (onWhere != null)
            {
                strJoin = " on " + onWhere.ToString();
            }

            string join = GetJoinEnumString(joinType);

            if (this.relation != null)
            {
                this.tableName = " {2} " + this.tableName;
                this.relation += " {3} ";
            }
            this.relation += join + from.TableName + strJoin;

            return this;
        }

        #endregion

        #endregion

        private string GetJoinEnumString(JoinType joinType)
        {
            switch (joinType)
            {
                case JoinType.LeftJoin:
                    return " left outer join ";
                case JoinType.RightJoin:
                    return " right outer join ";
                case JoinType.InnerJoin:
                    return " inner join ";
                default:
                    return " inner join ";
            }
        }

        #region 公有方法

        internal Field[] GetSelectFields()
        {
            IDictionary<string, Field> dictfields = new Dictionary<string, Field>();
            List<Field> fieldlist = new List<Field>();

            foreach (Entity entity in this.entityList)
            {
                Table table = entity.GetTable();
                Field[] fields = entity.GetFields();
                if (fields == null || fields.Length == 0)
                {
                    throw new MySoftException("没有任何被选中的字段列表！");
                }
                else
                {
                    foreach (Field field in fields)
                    {
                        if (!dictfields.ContainsKey(field.OriginalName))
                        {
                            if (table.AliasName != null)
                            {
                                dictfields.Add(field.OriginalName, field.At(table.AliasName));
                            }
                            else
                            {
                                dictfields.Add(field.OriginalName, field);
                            }
                        }
                    }
                }
            }

            //遍历字典，将Field取出
            foreach (KeyValuePair<string, Field> kv in dictfields)
            {
                fieldlist.Add(kv.Value);
            }

            return fieldlist.ToArray();

        }

        #endregion

        #region 内部变量

        private string tableName;
        internal string TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                tableName = value;
            }
        }

        private string relation;
        internal string Relation
        {
            get
            {
                return relation;
            }
            set
            {
                relation = value;
            }
        }

        #endregion
    }
}
