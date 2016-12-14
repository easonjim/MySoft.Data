using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using MySoft.Data;

namespace MySoft.Data
{
    public partial class DbSession : IDbSession
    {
        public static DbSession Default;
        private DbProvider dbProvider;
        private DbTrans dbTrans;

        #region 初始化Session

        static DbSession()
        {
            if (Default == null)
            {
                try
                {
                    Default = new DbSession(ProviderFactory.Default);
                }
                catch { }
            }
        }

        /// <summary>
        ///  指定配制节名实例化一个Session会话
        /// </summary>
        /// <param name="connectName"></param>
        public DbSession(string connectName)
            : this(ProviderFactory.CreateDbProvider(connectName))
        { }

        /// <summary>
        /// 指定驱动实例化一个Session会话
        /// </summary>
        /// <param name="dbProvider"></param>
        public DbSession(DbProvider dbProvider)
        {
            try
            {
                InitSession(dbProvider);
            }
            catch
            {
                throw new MySoftException("初始化DbSession失败，请检查配置是否正确！");
            }
        }

        /// <summary>
        /// 设置指定配制节名Session会话为默认会话
        /// </summary>
        /// <param name="connectName"></param>
        public static void SetDefault(string connectName)
        {
            Default = new DbSession(connectName);
        }

        /// <summary>
        /// 设置指定驱动Session会话为默认会话
        /// </summary>
        /// <param name="dbProvider"></param>
        public static void SetDefault(DbProvider dbProvider)
        {
            Default = new DbSession(dbProvider);
        }

        #endregion

        #region 实现IDbSession

        /// <summary>
        /// 设置新的驱动
        /// </summary>
        /// <param name="connectName"></param>
        public void SetProvider(string connectName)
        {
            SetProvider(ProviderFactory.CreateDbProvider(connectName));
        }

        /// <summary>
        /// 设置新的驱动
        /// </summary>
        /// <param name="dbProvider"></param>
        public void SetProvider(DbProvider dbProvider)
        {
            InitSession(dbProvider);
        }

        /// <summary>
        /// 开始一个事务
        /// </summary>
        /// <returns></returns>
        public DbTrans BeginTrans()
        {
            return new DbTrans(dbProvider, true);
        }

        /// <summary>
        /// 开始一个事务
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public DbTrans BeginTrans(IsolationLevel isolationLevel)
        {
            return new DbTrans(dbProvider, isolationLevel);
        }

        /// <summary>
        /// 设置一个外部事务
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public DbTrans SetTransaction(DbTransaction trans)
        {
            return new DbTrans(dbProvider, trans);
        }

        /// <summary>
        /// 开始一个外部事务
        /// </summary>
        /// <returns></returns>
        public DbTransaction BeginTransaction()
        {
            return BeginTrans().Transaction;
        }

        /// <summary>
        /// 开始一个外部事务
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return BeginTrans(isolationLevel).Transaction;
        }

        /// <summary>
        /// 设置一个外部链接
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public DbTrans SetConnection(DbConnection connection)
        {
            return new DbTrans(dbProvider, connection);
        }

        /// <summary>
        /// 创建一个外部连接
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            return dbProvider.CreateConnection();
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            return dbProvider.CreateParameter();
        }

        /// <summary>
        /// 解密ConnectionString
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected virtual string Decrypt(string connectionString)
        {
            //添加解密的方式
            return connectionString;
        }

        #region 缓存操作

        /// <summary>
        /// 开启Cache
        /// </summary>
        public void CacheOn()
        {
            if (dbProvider.CacheConfigSection != null)
            {
                dbProvider.CacheConfigSection.Enable = true;
            }
        }

        /// <summary>
        /// 关闭Cache
        /// </summary>
        public void CacheOff()
        {
            if (dbProvider.CacheConfigSection != null)
            {
                dbProvider.CacheConfigSection.Enable = false;
            }
        }

        /// <summary>
        /// 移除entityType对应cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveCache<T>()
        {
            string entityName = typeof(T).Name;
            dbProvider.RemoveCache(entityName);
        }

        /// <summary>
        /// 移除所有cache
        /// </summary>
        public void RemoveAllCache()
        {
            dbProvider.RemoveAllCache();
        }

        #endregion

        #region 注册Log

        /// <summary>
        /// 注册一个日志事件
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterSqlLogger(LogHandler handler)
        {
            dbProvider.OnLog += handler;
        }

        /// <summary>
        /// 取消一个日志事件
        /// </summary>
        /// <param name="handler"></param>
        public void UnregisterSqlLogger(LogHandler handler)
        {
            dbProvider.OnLog -= handler;
        }

        /// <summary>
        /// 注册一个日志事件
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterSqlExceptionLogger(ExceptionLogHandler handler)
        {
            dbProvider.OnExceptionLog += handler;
        }

        /// <summary>
        /// 取消一个日志事件
        /// </summary>
        /// <param name="handler"></param>
        public void UnregisterSqlExceptionLogger(ExceptionLogHandler handler)
        {
            dbProvider.OnExceptionLog -= handler;
        }

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
            return dbTrans.Single<T>(table, pkValues);
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
            return dbTrans.Single<T>(table, where);
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
            return dbTrans.Exists<T>(table, entity);
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
            return dbTrans.Exists<T>(table, pkValues);
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
            return dbTrans.Exists<T>(table, where);
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
            return dbTrans.Count<T>(table, where);
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
            return dbTrans.Sum<T>(table, field, where);
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
            return dbTrans.Avg<T>(table, field, where);
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
            return dbTrans.Max<T>(table, field, where);
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
            return dbTrans.Min<T>(table, field, where);
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
            return dbTrans.Sum<T, TResult>(table, field, where);
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
            return dbTrans.Avg<T, TResult>(table, field, where);
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
            return dbTrans.Max<T, TResult>(table, field, where);
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
            return dbTrans.Min<T, TResult>(table, field, where);
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
            return dbTrans.Single<T>(pkValues);
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
            return dbTrans.Single<T>(where);
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
            return dbTrans.Exists<T>(entity);
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
            return dbTrans.Exists<T>(pkValues);
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
            return dbTrans.Exists<T>(where);
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
            return dbTrans.Count<T>(where);
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
            return dbTrans.Sum<T>(field, where);
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
            return dbTrans.Avg<T>(field, where);
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
            return dbTrans.Max<T>(field, where);
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
            return dbTrans.Min<T>(field, where);
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
            return dbTrans.Sum<T, TResult>(field, where);
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
            return dbTrans.Avg<T, TResult>(field, where);
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
            return dbTrans.Max<T, TResult>(field, where);
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
            return dbTrans.Min<T, TResult>(field, where);
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
            return dbTrans.From<T>();
        }

        /// <summary>
        /// 返回一个From节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FromSection<T> From<T>(Table table)
            where T : Entity
        {
            return dbTrans.From<T>(table);
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
            return dbTrans.From<T>(aliasName);
        }

        /// <summary>
        /// 返回一个From节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FromSection<T> From<T>(TableRelation<T> relation)
            where T : Entity
        {
            return dbTrans.From<T>(relation);
        }

        /// <summary>
        /// 返回一个Sql节
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SqlSection FromSql(string sql, params SQLParameter[] parameters)
        {
            return dbTrans.FromSql(sql, parameters);
        }

        /// <summary>
        /// 返回一个Proc节
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public ProcSection FromProc(string procName, params SQLParameter[] parameters)
        {
            return dbTrans.FromProc(procName, parameters);
        }

        /// <summary>
        /// 返回一个Sql节
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SqlSection FromSql(string sql, IDictionary<string, object> parameters)
        {
            return dbTrans.FromSql(sql, parameters);
        }

        /// <summary>
        /// 返回一个Proc节
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public ProcSection FromProc(string procName, IDictionary<string, object> parameters)
        {
            return dbTrans.FromProc(procName, parameters);
        }

        #endregion

        #region 使用创建器操作

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public int Excute(InsertCreator creator)
        {
            return dbTrans.Excute(creator);
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
            return dbTrans.Excute(creator, out identityValue);
        }

        /// <summary>
        /// 按条件删除指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="creator"></param>
        /// <returns></returns>
        public int Excute(DeleteCreator creator)
        {
            return dbTrans.Excute(creator);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public int Excute(UpdateCreator creator)
        {
            return dbTrans.Excute(creator);
        }

        /// <summary>
        ///  返回一个Query节
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public QuerySection From(QueryCreator creator)
        {
            return dbTrans.From(creator);
        }

        #endregion

        #region 增删改操作

        /// <summary>
        /// 返回一个Batch
        /// </summary>
        /// <returns></returns>
        public DbBatch BeginBatch()
        {
            return dbTrans.BeginBatch();
        }

        /// <summary>
        /// 返回一个Batch
        /// </summary>
        /// <returns></returns>
        public DbBatch BeginBatch(int batchSize)
        {
            return dbTrans.BeginBatch(batchSize);
        }

        /// <summary>
        /// 保存一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Save<T>(Table table, T entity)
             where T : Entity
        {
            return dbTrans.Save<T>(table, entity);
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
            return dbTrans.Insert<T>(table, fields, values);
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
            return dbTrans.Insert<T, TResult>(table, fields, values, out retVal);
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
            return dbTrans.Delete<T>(table, entity);
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
            return dbTrans.Delete<T>(table, pkValues);
        }

        /// <summary>
        /// 删除符合条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<T>(Table table, WhereClip where)
            where T : Entity
        {
            return dbTrans.Delete<T>(table, where);
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
            return dbTrans.InsertOrUpdate<T>(table, entity);
        }

        /// <summary>
        /// 更新指定条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<T>(Table table, Field field, object value, WhereClip where)
            where T : Entity
        {
            return dbTrans.Update<T>(table, field, value, where);
        }

        /// <summary>
        /// 更新指定条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<T>(Table table, Field[] fields, object[] values, WhereClip where)
            where T : Entity
        {
            return dbTrans.Update<T>(table, fields, values, where);
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
            return dbTrans.Save(entity);
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
            return dbTrans.Insert<T>(fields, values);
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
            return dbTrans.Insert<T, TResult>(fields, values, out retVal);
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
            return dbTrans.Delete<T>(entity);
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
            return dbTrans.Delete<T>(pkValues);
        }

        /// <summary>
        /// 按条件删除指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<T>(WhereClip where)
            where T : Entity
        {
            return dbTrans.Delete<T>(where);
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
            return dbTrans.InsertOrUpdate<T>(entity);
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
            return dbTrans.Update<T>(field, value, where);
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
            return dbTrans.Update<T>(fields, values, where);
        }

        #endregion

        #region 系列化WhereClip

        /// <summary>
        /// 返回最终条件的SQL
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public string Serialization(WhereClip where)
        {
            string sql = dbProvider.Serialization(where.ToString());
            foreach (SQLParameter p in where.Parameters)
            {
                sql = sql.Replace(p.Name, DataUtils.FormatValue(p.Value));
            }
            return sql;
        }

        /// <summary>
        /// 返回最终排序的SQL
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string Serialization(OrderByClip order)
        {
            return dbProvider.Serialization(order.ToString());
        }

        #endregion

        #region 私有方法

        private void InitSession(DbProvider dbProvider)
        {
            this.dbProvider = dbProvider;
            this.dbProvider.SetEventHandler(Decrypt);
            this.dbProvider.DataCache = new DataCache();
            this.dbTrans = new DbTrans(dbProvider, false);

            #region 加载缓存配置
            try
            {
                object cacheConfig = ConfigurationManager.GetSection("cacheConfig");
                if (cacheConfig != null)
                {
                    CacheConfigurationSection config = (CacheConfigurationSection)cacheConfig;
                    IDictionary<string, CacheConfigInfo> configMap = new Dictionary<string, CacheConfigInfo>();

                    //获取缓存配制
                    foreach (string key in config.CacheEntities.AllKeys)
                    {
                        if (key.Contains("."))
                        {
                            string[] splittedKey = key.Split('.');
                            if (splittedKey[0] == this.dbProvider.ConnectName)
                            {
                                int expireSeconds = CacheConfigurationSection.DEFAULT_EXPIRE_SECONDS;
                                try
                                {
                                    expireSeconds = int.Parse(config.CacheEntities[key].Value);
                                }
                                catch { }

                                string entityName = splittedKey[1].Trim();
                                CacheConfigInfo cacheInfo = new CacheConfigInfo();
                                cacheInfo.EntityName = entityName;
                                cacheInfo.ExpireSeconds = expireSeconds;
                                configMap.Add(entityName, cacheInfo);
                            }
                        }
                    }

                    //检测缓存依赖关系
                    foreach (string key in config.CacheRelations.AllKeys)
                    {
                        if (key.Contains("."))
                        {
                            string[] splittedKey = key.Split('.');
                            if (splittedKey[0] == this.dbProvider.ConnectName)
                            {
                                string cacheKey = config.CacheRelations[key].Value;
                                string entityName = splittedKey[1].ToLower().Trim();

                                string[] relationKeys = cacheKey.Split(',');

                                if (configMap.ContainsKey(entityName))
                                {
                                    CacheConfigInfo cache = configMap[entityName];
                                    foreach (string relationKey in relationKeys)
                                    {
                                        if (!cache.RelationList.Contains(relationKey)) cache.RelationList.Add(relationKey);
                                    }
                                    configMap[entityName] = cache;
                                }
                            }
                        }
                    }

                    this.dbProvider.CacheConfigSection = config;
                    this.dbProvider.CacheConfigMap = configMap;
                }
            }
            catch
            {
                throw new MySoftException("CacheConfig配置加载失败！");
            }
            #endregion
        }

        #endregion

        #endregion
    }
}
