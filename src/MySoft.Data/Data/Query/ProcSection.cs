using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    /// <summary>
    /// �洢���̲���ʵ����
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
        /// ���Ӷ����������ǰProc����
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ProcSection AddParameters(params DbParameter[] parameters)
        {
            dbProvider.AddParameter(dbCommand, parameters);
            return this;
        }

        /// <summary>
        /// ���Ӷ����������ǰProc����
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ProcSection AddParameters(params SQLParameter[] parameters)
        {
            dbProvider.AddParameter(dbCommand, parameters);
            return this;
        }

        /// <summary>
        /// ���Ӷ����������ǰSql����
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
        /// ����һ�������������ǰProc����
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
        /// ����һ�������������ǰProc����
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
        /// ����һ�������������ǰProc����
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
        /// ����һ�����������������ǰProc����
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
        /// ����һ�����ز�������ǰProc����
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ProcSection AddReturnValueParameter(string parameterName, DbType dbType)
        {
            dbProvider.AddReturnValueParameter(dbCommand, parameterName, dbType);
            return this;
        }

        #region �����������

        /// <summary>
        /// ִ�е�ǰProc
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            return dbProvider.ExecuteNonQuery(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰProc������һ��ʵ��
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
        /// ִ�е�ǰProc������һ���б�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SourceList<T> ToList<T>()
            where T : Entity
        {
            return GetList<T>(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰProc������һ���б�
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public ArrayList<TResult> ToListResult<TResult>()
        {
            return GetListResult<TResult>(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰProc������һ��DbReader
        /// </summary>
        /// <returns></returns>
        public SourceReader ToReader()
        {
            return dbProvider.ExecuteReader(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰProc������һ��DataTable
        /// </summary>
        /// <returns></returns>
        public SourceTable ToTable()
        {
            DataTable dt = dbProvider.ExecuteDataTable(dbCommand, dbTran);
            return new SourceTable(dt);
        }

        /// <summary>
        /// ִ�е�ǰSql�������һ��DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ToDataSet()
        {
            return dbProvider.ExecuteDataSet(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰProc������һ��ֵ
        /// </summary>
        /// <returns></returns>
        public object ToScalar()
        {
            return dbProvider.ExecuteScalar(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰProc������һ��ֵ
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult ToScalar<TResult>()
        {
            object obj = this.ToScalar();
            return DataUtils.ConvertValue<TResult>(obj);
        }
        #endregion

        #region ���������

        /// <summary>
        /// ִ�е�ǰProc,���������ֵ
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
        /// ִ�е�ǰProc����һ��ʵ��,���������ֵ
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
        /// ִ�е�ǰProc����һ���б�,���������ֵ
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
        /// ִ�е�ǰProc������һ���б�
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
        /// ִ�е�ǰProc����һ��DbReader,���������ֵ
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
        /// ִ�е�ǰProc����һ��DataTable,���������ֵ
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
        /// ִ�е�ǰProc����һ��DataSet,���������ֵ
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
        /// ִ�е�ǰProc����һ��ֵ,���������ֵ
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
        /// ִ�е�ǰProc����һ��ֵ,���������ֵ
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

        #region ˽�з���

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
                    //������������ֱ������
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
