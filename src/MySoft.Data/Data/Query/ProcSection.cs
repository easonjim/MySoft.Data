using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    /// <summary>
    /// 存储过程操作实现类
    /// </summary>
    public class ProcSection : IProcSection
    {
        private DbProvider dbProvider;
        private DbCommand dbCommand;
        private DbTrans dbTran;

        internal ProcSection(string procName, DbProvider dbProvider, DbTrans dbTran)
        {
            this.dbProvider = dbProvider;
            this.dbTran = dbTran;
            this.dbCommand = dbProvider.CreateProcCommand(procName);
        }

        /// <summary>
        /// 增加多个参数到当前Proc命令
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ProcSection AddParameters(params DbParameter[] parameters)
        {
            dbProvider.AddParameter(dbCommand, parameters);
            return this;
        }

        /// <summary>
        /// 增加多个参数到当前Proc命令
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ProcSection AddParameters(params SQLParameter[] parameters)
        {
            dbProvider.AddParameter(dbCommand, parameters);
            return this;
        }

        /// <summary>
        /// 增加多个参数到当前Sql命令
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ProcSection AddParameters(IDictionary<string, object> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                List<SQLParameter> list = new List<SQLParameter>();
                foreach (KeyValuePair<string, object> kv in parameters)
                {
                    list.Add(new SQLParameter(kv.Key, kv.Value));
                }
                return AddParameters(list.ToArray());
            }

            return this;
        }

        /// <summary>
        /// 增加一个输入参数到当前Proc命令
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ProcSection AddInputParameter(string parameterName, DbType dbType, object value)
        {
            dbProvider.AddInputParameter(dbCommand, parameterName, dbType, value);
            return this;
        }

        /// <summary>
        /// 增加一个输入参数到当前Proc命令
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ProcSection AddInputParameter(string parameterName, DbType dbType, int size, object value)
        {
            dbProvider.AddInputParameter(dbCommand, parameterName, dbType, size, value);
            return this;
        }

        /// <summary>
        /// 增加一个输出参数到当前Proc命令
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ProcSection AddOutputParameter(string parameterName, DbType dbType, int size)
        {
            dbProvider.AddOutputParameter(dbCommand, parameterName, dbType, size);
            return this;
        }

        /// <summary>
        /// 增加一个输入输出参数到当前Proc命令
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ProcSection AddInputOutputParameter(string parameterName, DbType dbType, object value, int size)
        {
            dbProvider.AddInputOutputParameter(dbCommand, parameterName, dbType, value, size);
            return this;
        }

        /// <summary>
        /// 增加一个返回参数到当前Proc命令
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ProcSection AddReturnValueParameter(string parameterName, DbType dbType)
        {
            dbProvider.AddReturnValueParameter(dbCommand, parameterName, dbType);
            return this;
        }

        #region 不带参数输出

        /// <summary>
        /// 执行当前Proc
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            return dbProvider.ExecuteNonQuery(dbCommand, dbTran);
        }

        /// <summary>
        /// 执行当前Proc并返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToSingle<T>()
            where T : Entity
        {
            ISourceList<T> list = GetList<T>(dbCommand, dbTran);
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
        /// 执行当前Proc并返回一个列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SourceList<T> ToList<T>()
            where T : Entity
        {
            return GetList<T>(dbCommand, dbTran);
        }

        /// <summary>
        /// 执行当前Proc并返回一个列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public ArrayList<TResult> ToListResult<TResult>()
        {
            return GetListResult<TResult>(dbCommand, dbTran);
        }

        /// <summary>
        /// 执行当前Proc并返回一个DbReader
        /// </summary>
        /// <returns></returns>
        public SourceReader ToReader()
        {
            return dbProvider.ExecuteReader(dbCommand, dbTran);
        }

        /// <summary>
        /// 执行当前Proc并返回一个DataTable
        /// </summary>
        /// <returns></returns>
        public SourceTable ToTable()
        {
            DataTable dt = dbProvider.ExecuteDataTable(dbCommand, dbTran);
            return new SourceTable(dt);
        }

        /// <summary>
        /// 执行当前Sql命令并返回一个DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ToDataSet()
        {
            return dbProvider.ExecuteDataSet(dbCommand, dbTran);
        }

        /// <summary>
        /// 执行当前Proc并返回一个值
        /// </summary>
        /// <returns></returns>
        public object ToScalar()
        {
            return dbProvider.ExecuteScalar(dbCommand, dbTran);
        }

        /// <summary>
        /// 执行当前Proc并返回一个值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult ToScalar<TResult>()
        {
            object obj = this.ToScalar();
            return DataUtils.ConvertValue<TResult>(obj);
        }
        #endregion

        #region 带参数输出

        /// <summary>
        /// 执行当前Proc,并输出参数值
        /// </summary>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public int Execute(out IDictionary<string, object> outValues)
        {
            int value = dbProvider.ExecuteNonQuery(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
            return value;
        }

        /// <summary>
        /// 执行当前Proc返回一个实体,并输出参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public T ToSingle<T>(out IDictionary<string, object> outValues)
            where T : Entity
        {
            ISourceList<T> list = GetList<T>(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
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
        /// 执行当前Proc返回一个列表,并输出参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public SourceList<T> ToList<T>(out IDictionary<string, object> outValues)
            where T : Entity
        {
            SourceList<T> list = GetList<T>(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
            return list;
        }

        /// <summary>
        /// 执行当前Proc并返回一个列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public ArrayList<TResult> ToListResult<TResult>(out IDictionary<string, object> outValues)
        {
            ArrayList<TResult> results = GetListResult<TResult>(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
            return results;
        }

        /// <summary>
        /// 执行当前Proc返回一个DbReader,并输出参数值
        /// </summary>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public SourceReader ToReader(out IDictionary<string, object> outValues)
        {
            SourceReader reader = dbProvider.ExecuteReader(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
            return reader;
        }

        /// <summary>
        /// 执行当前Proc返回一个DataTable,并输出参数值
        /// </summary>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public SourceTable ToTable(out IDictionary<string, object> outValues)
        {
            DataTable dataTable = dbProvider.ExecuteDataTable(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
            return new SourceTable(dataTable);
        }

        /// <summary>
        /// 执行当前Proc返回一个DataSet,并输出参数值
        /// </summary>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public DataSet ToDataSet(out IDictionary<string, object> outValues)
        {
            DataSet dataSet = dbProvider.ExecuteDataSet(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
            return dataSet;
        }

        /// <summary>
        /// 执行当前Proc返回一个值,并输出参数值
        /// </summary>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public object ToScalar(out IDictionary<string, object> outValues)
        {
            object obj = dbProvider.ExecuteScalar(dbCommand, dbTran);
            GetOutputParameterValues(dbCommand, out outValues);
            return obj;
        }

        /// <summary>
        /// 执行当前Proc返回一个值,并输出参数值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outValues"></param>
        /// <returns></returns>
        public TResult ToScalar<TResult>(out IDictionary<string, object> outValues)
        {
            object obj = this.ToScalar(out outValues);
            return DataUtils.ConvertValue<TResult>(obj);
        }

        #endregion

        #region 私有方法

        private SourceList<T> GetList<T>(DbCommand cmd, DbTrans dbTran)
            where T : Entity
        {
            try
            {
                using (ISourceReader reader = dbProvider.ExecuteReader(cmd, dbTran))
                {
                    SourceList<T> list = new SourceList<T>();
                    FastCreateInstanceHandler creator = DataUtils.GetFastInstanceCreator(typeof(T));

                    while (reader.Read())
                    {
                        T entity = (T)creator();
                        entity.SetAllValues(reader);
                        entity.Attach();
                        list.Add(entity);
                    }

                    reader.Close();

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        private ArrayList<TResult> GetListResult<TResult>(DbCommand cmd, DbTrans dbTran)
        {
            try
            {
                using (ISourceReader reader = dbProvider.ExecuteReader(cmd, dbTran))
                {
                    ArrayList<TResult> list = new ArrayList<TResult>();

                    if (typeof(TResult) == typeof(object[]))
                    {
                        while (reader.Read())
                        {
                            List<object> objs = new List<object>();
                            for (int row = 0; row < reader.FieldCount; row++)
                            {
                                objs.Add(reader.GetValue(row));
                            }

                            TResult result = (TResult)(objs.ToArray() as object);
                            list.Add(result);
                        }
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            list.Add(reader.GetValue<TResult>(0));
                        }
                    }

                    reader.Close();

                    return list;
                }
            }
            catch
            {
                throw;
            }
        }

        private void GetOutputParameterValues(DbCommand cmd, out IDictionary<string, object> outValues)
        {
            try
            {
                IDictionary<string, object> returnValues = new Dictionary<string, object>();
                foreach (DbParameter p in cmd.Parameters)
                {
                    //如果是输出参数直接跳过
                    if (p.Direction == ParameterDirection.Input) continue;
                    if (p.Value == DBNull.Value) p.Value = null;

                    returnValues.Add(p.ParameterName.Substring(1), p.Value);
                }
                outValues = returnValues;
            }
            catch
            {
                throw;
            }
        }

        #endregion

    }
}
