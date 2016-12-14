using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    /// <summary>
    /// sql������ʵ����
    /// </summary>
    public class SqlSection : ISqlSection
    {
        private DbProvider dbProvider;
        private DbCommand dbCommand;
        private DbTrans dbTran;

        internal SqlSection(string sql, DbProvider dbProvider, DbTrans dbTran)
        {
            this.dbProvider = dbProvider;
            this.dbTran = dbTran;
            this.dbCommand = dbProvider.CreateSqlCommand(sql);
        }

        /// <summary>
        /// ���Ӷ����������ǰSql����
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSection AddParameters(params DbParameter[] parameters)
        {
            dbProvider.AddParameter(dbCommand, parameters);
            return this;
        }

        /// <summary>
        /// ���Ӷ����������ǰSql����
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSection AddParameters(params SQLParameter[] parameters)
        {
            dbProvider.AddParameter(dbCommand, parameters);
            return this;
        }

        /// <summary>
        /// ���Ӷ����������ǰSql����
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSection AddParameters(IDictionary<string, object> parameters)
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
        /// ����һ�������������ǰSql����
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlSection AddInputParameter(string paramName, DbType dbType, object value)
        {
            dbProvider.AddInputParameter(dbCommand, paramName, dbType, value);
            return this;
        }

        /// <summary>
        /// ����һ�������������ǰSql����
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlSection AddInputParameter(string paramName, DbType dbType, int size, object value)
        {
            dbProvider.AddInputParameter(dbCommand, paramName, dbType, size, value);
            return this;
        }

        /// <summary>
        /// ִ�е�ǰSql����
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            return dbProvider.ExecuteNonQuery(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰSql�������һ��ʵ��
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
        /// ִ�е�ǰSql�������һ��List�б�
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
        /// ִ�е�ǰSql�������һ��DbReader
        /// </summary>
        /// <returns></returns>
        public SourceReader ToReader()
        {
            return dbProvider.ExecuteReader(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰSql�������һ��DataTable
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
        /// ִ�е�ǰSql�������һ��ֵ
        /// </summary>
        /// <returns></returns>
        public object ToScalar()
        {
            return dbProvider.ExecuteScalar(dbCommand, dbTran);
        }

        /// <summary>
        /// ִ�е�ǰ
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult ToScalar<TResult>()
        {
            object obj = this.ToScalar();
            if (obj == null) return default(TResult);
            return DataUtils.ConvertValue<TResult>(obj);
        }

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

        #endregion
    }
}
