using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.Common;
using MySoft.Data;
using MySoft.Data.Design;

namespace MySoft.Data
{
    /// <summary>
    /// 事务处理类
    /// </summary>
    public class DbTrans : IDbTrans, IDisposable
    {
        private DbConnection dbConnection;
        private DbTransaction dbTransaction;
        private DbProvider dbProvider;
        private DbBatch dbBatch;

        internal DbConnection Connection
        {
            get { return this.dbConnection; }
        }

        internal DbTransaction Transaction
        {
            get { return this.dbTransaction; }
        }

        /// <summary>
        /// 以DbTransaction方式实例化一个事务
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="dbTran"></param>
        internal DbTrans(DbProvider dbProvider, DbTransaction dbTran)
        {
            this.dbConnection = dbTran.Connection;
            this.dbTransaction = dbTran;
            if (this.dbConnection.State != ConnectionState.Open)
            {
                this.dbConnection.Open();
            }
            this.dbProvider = dbProvider;
            this.dbBatch = new DbBatch(dbProvider, this);
        }

        /// <summary>
        /// 以BbConnection方式实例化一个事务
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="dbConnection"></param>
        internal DbTrans(DbProvider dbProvider, DbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            if (this.dbConnection.State != ConnectionState.Open)
            {
                this.dbConnection.Open();
            }
            this.dbProvider = dbProvider;
            this.dbBatch = new DbBatch(dbProvider, this);
        }

        internal DbTrans(DbProvider dbProvider, bool useTrans)
        {
            if (useTrans)
            {
                this.dbConnection = dbProvider.CreateConnection();
                this.dbConnection.Open();
                this.dbTransaction = dbConnection.BeginTransaction();
            }
            this.dbProvider = dbProvider;
            this.dbBatch = new DbBatch(dbProvider, this);
        }

        internal DbTrans(DbProvider dbProvider, IsolationLevel isolationLevel)
        {
            this.dbConnection = dbProvider.CreateConnection();
            this.dbConnection.Open();
            this.dbTransaction = dbConnection.BeginTransaction(isolationLevel);
            this.dbProvider = dbProvider;
            this.dbBatch = new DbBatch(dbProvider, this);
        }

        #region Batch操作

        /// <summary>
        /// 返回一个Batch
        /// </summary>
        /// <returns></returns>
        public DbBatch BeginBatch()
        {
            return BeginBatch(10);
        }

        /// <summary>
        /// 返回一个Batch
        /// </summary>
        /// <param name="batchSize">Batch大小</param>
        /// <returns></returns>
        public DbBatch BeginBatch(int batchSize)
        {
            return new DbBatch(dbProvider, this, batchSize);
        }

        #endregion

        #region Trans操作

        #region 增删改操作(指定表名)

        /// <summary>
        /// 保存一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Save<T>(Table table, T entity)
            where T : Entity
        {
            return dbBatch.Save<T>(table, entity);
        }

        /// <summary>
        ///  插入一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int Insert<T>(Table table, Field[] fields, object[] values)
            where T : Entity
        {
            return dbBatch.Insert<T>(table, fields, values);
        }

        /// <summary>
        ///  插入一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int Insert<T, TResult>(Table table, Field[] fields, object[] values, out TResult retVal)
            where T : Entity
        {
            return dbBatch.Insert<T, TResult>(table, fields, values, out retVal);
        }

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete<T>(Table table, T entity)
             where T : Entity
        {
            return dbBatch.Delete<T>(table, entity);
        }

        /// <summary>
        /// 删除指定主键的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public int Delete<T>(Table table, params object[] pkValues)
            where T : Entity
        {
            return dbBatch.Delete<T>(table, pkValues);
        }

        #endregion

        #region 增删改操作

        /// <summary>
        /// 保存一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Save<T>(T entity)
            where T : Entity
        {
            return dbBatch.Save(entity);
        }

        /// <summary>
        ///  插入一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int Insert<T>(Field[] fields, object[] values)
            where T : Entity
        {
            return dbBatch.Insert<T>(fields, values);
        }

        /// <summary>
        ///  插入一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int Insert<T, TResult>(Field[] fields, object[] values, out TResult retVal)
            where T : Entity
        {
            return dbBatch.Insert<T, TResult>(fields, values, out retVal);
        }

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete<T>(T entity)
             where T : Entity
        {
            return dbBatch.Delete<T>(entity);
        }

        /// <summary>
        /// 按主键删除指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public int Delete<T>(params object[] pkValues)
            where T : Entity
        {
            return dbBatch.Delete<T>(pkValues);
        }

        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int InsertOrUpdate<T>(T entity)
            where T : Entity
        {
            return dbBatch.InsertOrUpdate<T>(entity);
        }

        #endregion

        #region 事务操作

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            try
            {
                dbTransaction.Commit();
            }
            catch
            {
                this.Close();
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            try
            {
                dbTransaction.Rollback();
            }
            catch
            {
                this.Close();
            }
        }

        /// <summary>
        /// Dispose事务
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// 关闭事务
        /// </summary>
        public void Close()
        {
            if (dbConnection.State != ConnectionState.Closed)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        #endregion

        #endregion

        #region 常用操作(指定表名)

        /// <summary>
        /// 按主键获取一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public T Single<T>(Table table, params object[] pkValues)
            where T : Entity
        {
            WhereClip where = DataUtils.GetPkWhere<T>(table, pkValues);
            return Single<T>(table, where);
        }

        /// <summary>
        /// 按条件获取一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public T Single<T>(Table table, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Where(where).ToSingle();
        }

        /// <summary>
        /// 是否存在指定的实体，按主键匹配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Exists<T>(Table table, T entity)
            where T : Entity
        {
            WhereClip where = DataUtils.GetPkWhere<T>(table, entity);
            return Exists<T>(table, where);
        }

        /// <summary>
        /// 是否存在指定主键的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public bool Exists<T>(Table table, params object[] pkValues)
            where T : Entity
        {
            WhereClip where = DataUtils.GetPkWhere<T>(table, pkValues);
            return Exists<T>(table, where);
        }

        /// <summary>
        /// 是否存在指定条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Exists<T>(Table table, WhereClip where)
            where T : Entity
        {
            return Count<T>(table, where) > 0;
        }

        /// <summary>
        /// 按条件获取记录条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Count<T>(Table table, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Where(where).Count();
        }

        /// <summary>
        /// 按条件进行Sum操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Sum<T>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Sum()).Where(where).ToScalar();
        }

        /// <summary>
        /// 按条件进行Avg操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Avg<T>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Avg()).Where(where).ToScalar();
        }

        /// <summary>
        /// 按条件进行Max操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Max<T>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Max()).Where(where).ToScalar();
        }

        /// <summary>
        /// 按条件进行Min操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Min<T>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Min()).Where(where).ToScalar();
        }

        #region 返回相应的类型

        /// <summary>
        /// 按条件进行Sum操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Sum<T, TResult>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Sum()).Where(where).ToScalar<TResult>();
        }

        /// <summary>
        /// 按条件进行Avg操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Avg<T, TResult>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Avg()).Where(where).ToScalar<TResult>();
        }

        /// <summary>
        /// 按条件进行Max操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Max<T, TResult>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Max()).Where(where).ToScalar<TResult>();
        }

        /// <summary>
        /// 按条件进行Min操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Min<T, TResult>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return From<T>(table).Select(field.Min()).Where(where).ToScalar<TResult>();
        }

        #endregion

        #endregion

        #region 常用操作

        /// <summary>
        /// 按主键获取一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public T Single<T>(params object[] pkValues)
            where T : Entity
        {
            return Single<T>(null, pkValues);
        }

        /// <summary>
        /// 按条件获取一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public T Single<T>(WhereClip where)
            where T : Entity
        {
            return Single<T>(null, where);
        }

        /// <summary>
        /// 是否存在指定的实体，按主键匹配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Exists<T>(T entity)
            where T : Entity
        {
            return Exists(null, entity);
        }

        /// <summary>
        /// 是否存在指定主键的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public bool Exists<T>(params object[] pkValues)
            where T : Entity
        {
            return Exists<T>(null, pkValues);
        }

        /// <summary>
        /// 是否存在指定条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Exists<T>(WhereClip where)
            where T : Entity
        {
            return Exists<T>(null, where);
        }

        /// <summary>
        /// 按条件获取记录条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Count<T>(WhereClip where)
            where T : Entity
        {
            return Count<T>(null, where);
        }

        /// <summary>
        /// 按条件进行Sum操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Sum<T>(Field field, WhereClip where)
            where T : Entity
        {
            return Sum<T>(null, field, where);
        }

        /// <summary>
        /// 按条件进行Avg操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Avg<T>(Field field, WhereClip where)
            where T : Entity
        {
            return Avg<T>(null, field, where);
        }

        /// <summary>
        /// 按条件进行Max操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Max<T>(Field field, WhereClip where)
            where T : Entity
        {
            return Max<T>(null, field, where);
        }

        /// <summary>
        /// 按条件进行Min操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public object Min<T>(Field field, WhereClip where)
            where T : Entity
        {
            return Min<T>(null, field, where);
        }

        #region 返回相应的类型

        /// <summary>
        /// 按条件进行Sum操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Sum<T, TResult>(Field field, WhereClip where)
            where T : Entity
        {
            return Sum<T, TResult>(null, field, where);
        }

        /// <summary>
        /// 按条件进行Avg操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Avg<T, TResult>(Field field, WhereClip where)
            where T : Entity
        {
            return Avg<T, TResult>(null, field, where);
        }

        /// <summary>
        /// 按条件进行Max操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Max<T, TResult>(Field field, WhereClip where)
            where T : Entity
        {
            return Max<T, TResult>(null, field, where);
        }

        /// <summary>
        /// 按条件进行Min操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public TResult Min<T, TResult>(Field field, WhereClip where)
            where T : Entity
        {
            return Min<T, TResult>(null, field, where);
        }

        #endregion

        #endregion

        #region 进行连接操作

        /// <summary>
        /// 返回一个From节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FromSection<T> From<T>()
            where T : Entity
        {
            return From<T>((string)null);
        }

        /// <summary>
        /// 返回一个From节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FromSection<T> From<T>(Table table)
            where T : Entity
        {
            return new FromSection<T>(dbProvider, this, table);
        }

        /// <summary>
        /// 返回一个From节，并可指定其别名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public FromSection<T> From<T>(string aliasName)
            where T : Entity
        {
            return new FromSection<T>(dbProvider, this, aliasName);
        }

        /// <summary>
        /// 返回一个From节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FromSection<T> From<T>(TableRelation<T> relation)
            where T : Entity
        {
            FromSection<T> section = From<T>();
            section.SetQuerySection(relation.Section.Query);

            //给查询设置驱动与事务
            section.Query.SetDbProvider(dbProvider, this);

            return section;
        }

        /// <summary>
        /// 返回一个Sql节
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SqlSection FromSql(string sql, params SQLParameter[] parameters)
        {
            SqlSection section = new SqlSection(sql, dbProvider, this);
            return section.AddParameters(parameters);
        }

        /// <summary>
        /// 返回一个Proc节
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public ProcSection FromProc(string procName, params SQLParameter[] parameters)
        {
            ProcSection section = new ProcSection(procName, dbProvider, this);
            return section.AddParameters(parameters);
        }

        /// <summary>
        /// 返回一个Sql节
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SqlSection FromSql(string sql, IDictionary<string, object> parameters)
        {
            SqlSection section = new SqlSection(sql, dbProvider, this);
            return section.AddParameters(parameters);
        }

        /// <summary>
        /// 返回一个Proc节
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public ProcSection FromProc(string procName, IDictionary<string, object> parameters)
        {
            ProcSection section = new ProcSection(procName, dbProvider, this);
            return section.AddParameters(parameters);
        }

        #endregion

        #region 按创建器操作

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public int Excute(InsertCreator creator)
        {
            if (creator.Table == null)
            {
                throw new MySoftException("用创建器操作时，表不能为null！");
            }

            object retVal;
            return dbProvider.Insert<TempTable>(creator.Table, creator.FieldValues, this, creator.IdentityField, creator.SequenceName, false, out retVal);
        }

        /// <summary>
        ///  插入数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="creator"></param>
        /// <param name="identityValue"></param>
        /// <returns></returns>
        public int Excute<TResult>(InsertCreator creator, out TResult identityValue)
        {
            identityValue = default(TResult);

            if (creator.Table == null)
            {
                throw new MySoftException("用创建器操作时，表不能为null！");
            }

            if ((IField)creator.IdentityField == null)
            {
                throw new MySoftException("返回主键值时需要设置KeyField！");
            }

            object retVal;
            int ret = dbProvider.Insert<TempTable>(creator.Table, creator.FieldValues, this, creator.IdentityField, creator.SequenceName, true, out retVal);
            identityValue = DataUtils.ConvertValue<TResult>(retVal);

            return ret;
        }

        /// <summary>
        /// 按条件删除指定记录
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public int Excute(DeleteCreator creator)
        {
            if (creator.Table == null)
            {
                throw new MySoftException("用创建器操作时，表不能为null！");
            }

            if (DataUtils.IsNullOrEmpty(creator.Where))
            {
                throw new MySoftException("用删除创建器操作时，条件不能为空！");
            }

            return Delete<TempTable>(creator.Table, creator.Where);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public int Excute(UpdateCreator creator)
        {
            if (creator.Table == null)
            {
                throw new MySoftException("用创建器操作时，表不能为null！");
            }

            if (DataUtils.IsNullOrEmpty(creator.Where))
            {
                throw new MySoftException("用更新创建器操作时，条件不能为空！");
            }

            return Update<TempTable>(creator.Table, creator.Fields, creator.Values, creator.Where);
        }

        /// <summary>
        /// 返回一个Query节
        /// </summary>
        /// <returns></returns>
        public QuerySection From(QueryCreator creator)
        {
            if (creator.Table == null)
            {
                throw new MySoftException("用创建器操作时，表不能为null！");
            }

            FromSection<TempTable> f = this.From<TempTable>(creator.Table);
            if (creator.IsRelation)
            {
                foreach (TableJoin join in creator.Relations.Values)
                {
                    if (join.Type == JoinType.LeftJoin)
                        f.LeftJoin<TempTable>(join.Table, join.Where);
                    else if (join.Type == JoinType.RightJoin)
                        f.RightJoin<TempTable>(join.Table, join.Where);
                    else
                        f.InnerJoin<TempTable>(join.Table, join.Where);
                }
            }

            QuerySection<TempTable> query = f.Select(creator.Fields).Where(creator.Where)
                    .OrderBy(creator.OrderBy);

            return new QuerySection(query);
        }

        #endregion

        #region 按条件操作

        /// <summary>
        /// 按条件删除指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<T>(WhereClip where)
            where T : Entity
        {
            return Delete<T>(null, where);
        }

        /// <summary>
        /// 按条件更新指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<T>(Field field, object value, WhereClip where)
            where T : Entity
        {
            return Update<T>(null, field, value, where);
        }

        /// <summary>
        /// 按条件更新指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<T>(Field[] fields, object[] values, WhereClip where)
            where T : Entity
        {
            return Update<T>(null, fields, values, where);
        }

        /// <summary>
        /// 按条件删除指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<T>(Table table, WhereClip where)
            where T : Entity
        {
            return dbProvider.Delete<T>(table, where, this);
        }

        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int InsertOrUpdate<T>(Table table, T entity)
            where T : Entity
        {
            return dbBatch.InsertOrUpdate<T>(table, entity);
        }

        /// <summary>
        /// 按条件更新指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<T>(Table table, Field field, object value, WhereClip where)
            where T : Entity
        {
            return Update<T>(table, new Field[] { field }, new object[] { value }, where);
        }

        /// <summary>
        /// 按条件更新指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<T>(Table table, Field[] fields, object[] values, WhereClip where)
            where T : Entity
        {
            List<FieldValue> fvlist = DataUtils.CreateFieldValue(fields, values, false);
            return dbProvider.Update<T>(table, fvlist, where, this);
        }

        #endregion
    }
}
