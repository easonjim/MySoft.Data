using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using MySoft.Data.Design;

namespace MySoft.Data
{
    public abstract class DbProvider : IDbProvider
    {
        private DbHelper dbHelper;
        private char leftToken;
        private char rightToken;
        private char paramPrefixToken;

        protected DbProvider(string connectionString, DbProviderFactory dbFactory, char leftToken, char rightToken, char paramPrefixToken)
        {
            this.connectName = "Default";
            this.leftToken = leftToken;
            this.rightToken = rightToken;
            this.paramPrefixToken = paramPrefixToken;
            this.dbHelper = new DbHelper(connectionString, dbFactory);
        }

        internal void SetEventHandler(DecryptEventHandler decryptEvent)
        {
            this.dbHelper.SetDecryptHandler(decryptEvent);
        }

        #region 保存Cache信息
        private string connectName;
        internal string ConnectName
        {
            get { return connectName; }
            set { connectName = value; }
        }

        private DataCache dataCache;
        internal DataCache DataCache
        {
            get { return dataCache; }
            set { dataCache = value; }
        }

        private CacheConfigurationSection cacheConfigSection = null;
        internal CacheConfigurationSection CacheConfigSection
        {
            get { return cacheConfigSection; }
            set { cacheConfigSection = value; }
        }

        private IDictionary<string, CacheConfigInfo> cacheConfigMap = new Dictionary<string, CacheConfigInfo>();
        internal IDictionary<string, CacheConfigInfo> CacheConfigMap
        {
            get { return cacheConfigMap; }
            set { cacheConfigMap = value; }
        }

        internal void RemoveCache(string entityName)
        {
            //数据有更新将移除缓存
            //同时需要移除有关联关系中的表缓存
            lock (cacheConfigMap)
            {
                if (cacheConfigMap.ContainsKey(entityName))
                {
                    dataCache.RemoveCache(cacheConfigMap[entityName]);
                }
            }
        }

        internal void RemoveAllCache()
        {
            dataCache.RemoveAllCache();
        }

        #endregion

        #region 增加DbCommand参数

        private string FormatParameter(string parameterName)
        {
            if (parameterName.IndexOf(paramPrefixToken) == 0) return parameterName;
            if (parameterName.IndexOf('$') == 0) return paramPrefixToken + parameterName.TrimStart('$');
            return paramPrefixToken + parameterName;
        }

        /// <summary>
        /// 给命令添加参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        public void AddParameter(DbCommand cmd, DbParameter[] parameters)
        {
            foreach (DbParameter p in parameters)
            {
                if (p.Value == null) p.Value = DBNull.Value;
                else if (p.Value.GetType().IsEnum)
                {
                    //对枚举进行特殊处理
                    p.Value = Convert.ToInt32(p.Value);
                }

                cmd.Parameters.Add(p);
            }
        }

        /// <summary>
        /// 给命令添加参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public void AddParameter(DbCommand cmd, SQLParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return;

            List<DbParameter> list = new List<DbParameter>();
            foreach (SQLParameter p in parameters)
            {
                DbParameter dbParameter = CreateParameter(p.Name, p.Value);
                dbParameter.Direction = p.Direction;

                list.Add(dbParameter);
            }

            AddParameter(cmd, list.ToArray());
        }

        public void AddInputParameter(DbCommand cmd, string parameterName, DbType dbType, int size, object value)
        {
            dbHelper.AddInputParameter(cmd, parameterName, dbType, size, value);
        }

        public void AddInputParameter(DbCommand cmd, string parameterName, DbType dbType, object value)
        {
            dbHelper.AddInputParameter(cmd, parameterName, dbType, value);
        }

        public void AddOutputParameter(DbCommand cmd, string parameterName, DbType dbType, int size)
        {
            dbHelper.AddOutputParameter(cmd, parameterName, dbType, size);
        }

        public void AddInputOutputParameter(DbCommand cmd, string parameterName, DbType dbType, object value, int size)
        {
            dbHelper.AddInputOutputParameter(cmd, parameterName, dbType, value, size);
        }

        public void AddReturnValueParameter(DbCommand cmd, string parameterName, DbType dbType)
        {
            dbHelper.AddReturnValueParameter(cmd, parameterName, dbType);
        }

        public DbParameter GetParameter(DbCommand cmd, string parameterName)
        {
            return dbHelper.GetParameter(cmd, parameterName);
        }

        #endregion

        #region 执行SQL语句

        public int ExecuteNonQuery(DbCommand cmd, DbTrans trans)
        {
            //调整DbCommand;
            PrepareCommand(cmd);

            //写日志
            WriteLog(cmd);

            try
            {
                if (trans.Connection == null && trans.Transaction == null)
                {
                    return dbHelper.ExecuteNonQuery(cmd);
                }
                return dbHelper.ExecuteNonQuery(cmd, trans);
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex, cmd);

                throw ex;
            }
        }

        public SourceReader ExecuteReader(DbCommand cmd, DbTrans trans)
        {
            //调整DbCommand;
            PrepareCommand(cmd);

            //写日志
            WriteLog(cmd);

            try
            {
                IDataReader reader;
                if (trans.Connection == null && trans.Transaction == null)
                {
                    reader = dbHelper.ExecuteReader(cmd);
                }
                else
                {
                    reader = dbHelper.ExecuteReader(cmd, trans);
                }
                return new SourceReader(reader);
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex, cmd);

                throw ex;
            }
        }

        public DataSet ExecuteDataSet(DbCommand cmd, DbTrans trans)
        {
            //调整DbCommand;
            PrepareCommand(cmd);

            //写日志
            WriteLog(cmd);

            try
            {
                if (trans.Connection == null && trans.Transaction == null)
                {
                    return dbHelper.ExecuteDataSet(cmd);
                }
                return dbHelper.ExecuteDataSet(cmd, trans);
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex, cmd);

                throw ex;
            }
        }

        public DataTable ExecuteDataTable(DbCommand cmd, DbTrans trans)
        {
            //调整DbCommand;
            PrepareCommand(cmd);

            //写日志
            WriteLog(cmd);

            try
            {
                if (trans.Connection == null && trans.Transaction == null)
                {
                    return dbHelper.ExecuteDataTable(cmd);
                }
                return dbHelper.ExecuteDataTable(cmd, trans);
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex, cmd);

                throw ex;
            }
        }

        public object ExecuteScalar(DbCommand cmd, DbTrans trans)
        {
            //调整DbCommand;
            PrepareCommand(cmd);

            //写日志
            WriteLog(cmd);

            try
            {
                if (trans.Connection == null && trans.Transaction == null)
                {
                    return dbHelper.ExecuteScalar(cmd);
                }
                return dbHelper.ExecuteScalar(cmd, trans);
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex, cmd);

                throw ex;
            }
        }

        #endregion

        #region 创建DbConnection及DbParameter

        /// <summary>
        /// 创建DbConnection
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            return dbHelper.CreateConnection();
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            return dbHelper.CreateParameter();
        }

        #endregion

        #region 公用增删改方法

        #region Insert

        internal int Insert<T>(Table table, List<FieldValue> fvlist, DbTrans trans, Field identityfield, string autoIncrementName, bool isOutValue, out object retVal)
            where T : Entity
        {
            retVal = null;
            T entity = DataUtils.CreateInstance<T>();

            int returnValue = 0;
            DbCommand cmd = CreateInsert<T>(table, fvlist, identityfield, autoIncrementName);
            string tableName = table == null ? entity.GetTable().Name : table.Name;

            if ((IField)identityfield != null)
            {
                //Access获取最大的记录号
                if (AccessProvider)
                {
                    returnValue = ExecuteNonQuery(cmd, trans);
                    cmd = CreateSqlCommand(string.Format(RowAutoID, identityfield.Name, tableName));

                    if (isOutValue) retVal = ExecuteScalar(cmd, trans);
                }
                else
                {
                    if (UseAutoIncrement)
                    {
                        returnValue = ExecuteNonQuery(cmd, trans);

                        if (isOutValue)
                        {
                            if (!string.IsNullOrEmpty(autoIncrementName))
                            {
                                cmd = CreateSqlCommand(string.Format(RowAutoID, autoIncrementName));
                                retVal = ExecuteScalar(cmd, trans);
                            }
                        }
                    }
                    else
                    {
                        if (isOutValue)
                        {
                            cmd.CommandText += ";" + string.Format(RowAutoID, identityfield.Name, tableName);
                            retVal = ExecuteScalar(cmd, trans);
                            returnValue = 1;
                        }
                        else
                        {
                            returnValue = ExecuteNonQuery(cmd, trans);
                        }
                    }
                }
            }
            else
            {
                returnValue = ExecuteNonQuery(cmd, trans);
            }

            return returnValue;
        }

        internal DbCommand CreateInsert<T>(Table table, List<FieldValue> fvlist, Field identityfield, string autoIncrementName)
            where T : Entity
        {
            T entity = DataUtils.CreateInstance<T>();

            if (entity.GetReadOnly())
            {
                throw new MySoftException("只读实体" + typeof(T).Name + "只能用于查询！");
            }

            //移除缓存
            RemoveCache(typeof(T).Name);

            string tableName = table == null ? entity.GetTable().Name : table.Name;

            List<SQLParameter> plist = new List<SQLParameter>();
            StringBuilder sbsql = new StringBuilder();
            StringBuilder sbparam = new StringBuilder();

            if (UseAutoIncrement)
            {
                //如果标识列和标识名称都不为null
                if ((IField)identityfield != null && !string.IsNullOrEmpty(autoIncrementName))
                {
                    string identityName = FormatIdentityName(autoIncrementName);
                    bool exist = false;
                    fvlist.ForEach(fv =>
                    {
                        if (fv.IsIdentity)
                        {
                            fv.Value = new SysValue(identityName);
                            fv.IsIdentity = false;
                            exist = true;
                        }
                    });

                    if (!exist)
                    {
                        object value = new SysValue(identityName);
                        FieldValue fv = new FieldValue(identityfield, value);
                        fvlist.Insert(0, fv);
                    }
                }
            }

            sbsql.Append("insert into " + tableName + "(");
            sbparam.Append(" values (");

            fvlist.ForEach(fv =>
            {
                if (fv.IsIdentity) return;

                sbsql.Append(fv.Field.At((string)null).Name);
                if (CheckValue(fv.Value))
                {
                    sbparam.Append(DataUtils.FormatValue(fv.Value));
                }
                else
                {
                    SQLParameter p = null;
                    if (CheckStruct(fv.Value))
                        p = CreateOrmParameter(DataUtils.FormatValue(fv.Value));
                    else
                        p = CreateOrmParameter(fv.Value);

                    sbparam.Append(p.Name);
                    plist.Add(p);
                }

                sbsql.Append(",");
                sbparam.Append(",");
            });

            sbsql.Remove(sbsql.Length - 1, 1).Append(")");
            sbparam.Remove(sbparam.Length - 1, 1).Append(")");

            string cmdText = string.Format("{0}{1}", sbsql, sbparam);

            return CreateSqlCommand(cmdText, plist.ToArray());
        }

        #endregion

        #region Delete

        internal int Delete<T>(Table table, WhereClip where, DbTrans trans)
            where T : Entity
        {
            DbCommand cmd = CreateDelete<T>(table, where);
            return ExecuteNonQuery(cmd, trans);
        }

        internal DbCommand CreateDelete<T>(Table table, WhereClip where)
            where T : Entity
        {
            T entity = DataUtils.CreateInstance<T>();

            if (entity.GetReadOnly())
            {
                throw new MySoftException("只读实体" + typeof(T).Name + "只能用于查询！");
            }

            if ((object)where == null)
            {
                throw new MySoftException("删除条件不能为null！");
            }

            //移除缓存
            RemoveCache(typeof(T).Name);

            StringBuilder sb = new StringBuilder();
            string tableName = table == null ? entity.GetTable().Name : table.Name;
            sb.Append("delete from " + tableName);

            if (!DataUtils.IsNullOrEmpty(where))
            {
                sb.Append(" where " + where.ToString());
            }

            return CreateSqlCommand(sb.ToString(), where.Parameters);
        }

        #endregion

        #region Update

        internal int Update<T>(Table table, List<FieldValue> fvlist, WhereClip where, DbTrans trans)
            where T : Entity
        {
            //如果没有设置更新的字段，返回-1
            if (fvlist.FindAll(fv => { return fv.IsChanged; }).Count == 0) return -1;

            DbCommand cmd = CreateUpdate<T>(table, fvlist, where);
            return ExecuteNonQuery(cmd, trans);
        }

        internal DbCommand CreateUpdate<T>(Table table, List<FieldValue> fvlist, WhereClip where)
            where T : Entity
        {
            T entity = DataUtils.CreateInstance<T>();

            if (entity.GetReadOnly())
            {
                throw new MySoftException("只读实体" + typeof(T).Name + "只能用于查询！");
            }

            if ((object)where == null)
            {
                throw new MySoftException("更新条件不能为null！");
            }

            //移除缓存
            RemoveCache(typeof(T).Name);

            string tableName = table == null ? entity.GetTable().Name : table.Name;

            List<SQLParameter> plist = new List<SQLParameter>();
            StringBuilder sb = new StringBuilder();

            sb.Append("update " + tableName + " set ");

            fvlist.ForEach(fv =>
            {
                if (fv.IsPrimaryKey || fv.IsIdentity) return;

                if (fv.IsChanged)
                {
                    if (CheckValue(fv.Value))
                    {
                        sb.Append(fv.Field.At((string)null).Name + " = " + DataUtils.FormatValue(fv.Value));
                    }
                    else
                    {
                        SQLParameter p = null;
                        if (CheckStruct(fv.Value))
                            p = CreateOrmParameter(DataUtils.FormatValue(fv.Value));
                        else
                            p = CreateOrmParameter(fv.Value);

                        sb.Append(fv.Field.At((string)null).Name + " = " + p.Name);
                        plist.Add(p);
                    }

                    sb.Append(",");
                }
            });

            sb.Remove(sb.Length - 1, 1);

            if (!DataUtils.IsNullOrEmpty(where))
            {
                sb.Append(" where " + where.ToString());
                plist.AddRange(where.Parameters);
            }

            return CreateSqlCommand(sb.ToString(), plist.ToArray());
        }

        #endregion

        /// <summary>
        /// 返回最终排序的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        internal protected string Serialization(string sql)
        {
            return DataUtils.FormatSQL(sql, leftToken, rightToken, AccessProvider);
        }

        internal DbCommand CreateSqlCommand(string cmdText)
        {
            return dbHelper.CreateSqlStringCommand(cmdText);
        }

        /// <summary>
        /// 创建SQL命令
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        internal DbCommand CreateSqlCommand(string cmdText, SQLParameter[] parameters)
        {
            DbCommand cmd = dbHelper.CreateSqlStringCommand(cmdText);
            AddParameter(cmd, parameters);

            return cmd;
        }

        /// <summary>
        /// 创建存储过程命令
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        internal DbCommand CreateProcCommand(string procName)
        {
            return dbHelper.CreateStoredProcCommand(procName);
        }

        /// <summary>
        /// 格式化命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal DbCommand FormatCommand(DbCommand cmd)
        {
            return PrepareCommand(cmd);
        }

        #endregion

        #region ILogable Members

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="command">The command.</param>
        private void WriteLog(DbCommand command)
        {
            if (OnLog != null)
            {
                OnLog(GetLog(command));
            }
        }

        /// <summary>
        /// Writes the exception log.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="command"></param>
        private void WriteExceptionLog(Exception ex, DbCommand command)
        {
            if (OnExceptionLog != null)
            {
                OnExceptionLog(ex, GetLog(command));
            }
        }

        /// <summary>
        /// 获取输出的日志
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual string GetLog(DbCommand command)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("{0}\t{1}\t\r\n", command.CommandType, command.CommandText));
            if (command.Parameters != null && command.Parameters.Count > 0)
            {
                sb.Append("Parameters:\r\n");
                foreach (DbParameter p in command.Parameters)
                {
                    if (p.Size > 0)
                        sb.Append(string.Format("{0}[{2}({3})] = {1}\r\n", p.ParameterName, p.Value, p.DbType, p.Size));
                    else
                        sb.Append(string.Format("{0}[{2}] = {1}\r\n", p.ParameterName, p.Value, p.DbType));
                }
            }
            sb.Append("\r\n");

            return sb.ToString();
        }

        /// <summary>
        /// OnLog event.
        /// </summary>
        public event LogHandler OnLog;

        /// <summary>
        /// OnExceptionLog event.
        /// </summary>
        public event ExceptionLogHandler OnExceptionLog;

        #endregion

        #region 需重写的方法

        /// <summary>
        /// 格式化IdentityName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual string FormatIdentityName(string name)
        {
            return name;
        }

        /// <summary>
        /// 调整DbCommand命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected virtual DbCommand PrepareCommand(DbCommand cmd)
        {
            cmd.CommandText = Serialization(cmd.CommandText);
            foreach (DbParameter p in cmd.Parameters)
            {
                string oldName = p.ParameterName;
                p.ParameterName = FormatParameter(p.ParameterName);

                if (cmd.CommandType == CommandType.Text)
                {
                    if (cmd.CommandText.Contains(oldName) && !cmd.CommandText.Contains(p.ParameterName))
                    {
                        cmd.CommandText = cmd.CommandText.Replace(oldName, p.ParameterName);
                    }
                }
            }
            return cmd;
        }

        /// <summary>
        /// 创建分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="itemCount"></param>
        /// <param name="skipCount"></param>
        /// <returns></returns>
        protected internal virtual QuerySection<T> CreatePageQuery<T>(QuerySection<T> query, int itemCount, int skipCount)
            where T : Entity
        {
            if (skipCount == 0)
            {
                ((IPaging)query).Prefix("top " + itemCount);
                return query;
            }
            else
            {
                ((IPaging)query).Prefix("top " + itemCount);

                Field pagingField = query.PagingField;

                if ((IField)pagingField == null)
                {
                    throw new MySoftException("SqlServer2000或Access请使用SetPagingField设定分页主键！");
                }

                QuerySection<T> jquery = query.CreateQuery<T>();
                ((IPaging)jquery).Prefix("top " + skipCount);
                jquery.Select(pagingField);

                //如果是联合查询，则需要符值整个QueryString
                if (query.UnionQuery)
                {
                    jquery.QueryString = query.QueryString;
                }

                query.PageWhere = !pagingField.In(jquery);

                return query;
            }
        }

        #endregion

        #region 抽像方法

        /// <summary>
        /// 是否为Access驱动
        /// </summary>
        protected virtual bool AccessProvider
        {
            get { return false; }
        }

        /// <summary>
        /// 是否使用自增列
        /// </summary>
        protected virtual bool UseAutoIncrement
        {
            get { return false; }
        }

        /// <summary>
        /// 是否支持批处理
        /// </summary>
        protected internal abstract bool SupportBatch { get; }

        /// <summary>
        /// 返回自动ID的sql语句
        /// </summary>
        protected abstract string RowAutoID { get; }

        /// <summary>
        /// 创建DbParameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        protected abstract DbParameter CreateParameter(string parameterName, object val);

        #endregion

        /// <summary>
        /// 创建OrmParameter
        /// </summary>
        /// <returns></returns>
        private SQLParameter CreateOrmParameter(object value)
        {
            string pName = DataUtils.MakeUniqueKey(30, "$p");
            return new SQLParameter(pName, value);
        }

        /// <summary>
        /// 检测是否为非法值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckValue(object value)
        {
            if (value == null || value == DBNull.Value || value is Field || value is SysValue)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检测是否为结构数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckStruct(object value)
        {
            //当属性为结构时进行系列化
            Type type = value.GetType();
            if (type.IsValueType && !type.IsEnum && !type.IsPrimitive && !type.IsSerializable)
            {
                return true;
            }
            return false;
        }
    }
}
