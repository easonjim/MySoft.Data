using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Threading;

namespace MySoft.Data
{
    /// <summary>
    /// 批处理
    /// </summary>
    public class DbBatch : IDbBatch
    {
        private bool useBatch = false;
        private int batchSize;
        private DbProvider dbProvider;
        private DbTrans dbTrans;
        private List<DbCommand> commandList = new List<DbCommand>();

        internal DbBatch(DbProvider dbProvider, DbTrans dbTran, int batchSize)
        {
            this.dbProvider = dbProvider;
            this.batchSize = batchSize > 0 ? batchSize : 1;
            this.dbTrans = dbTran;
            this.useBatch = true;
        }

        internal DbBatch(DbProvider dbProvider, DbTrans dbTran)
        {
            this.dbProvider = dbProvider;
            this.dbTrans = dbTran;
            this.useBatch = false;
        }

        #region Trans操作

        /// <summary>
        /// 执行批处理操作
        /// </summary>
        /// <returns></returns>
        public int Process()
        {
            IList<MySoftException> errors;
            return Process(out errors);
        }

        /// <summary>
        /// 执行批处理操作
        /// </summary>
        /// <param name="errors">输出的错误</param>
        /// <returns></returns>
        public int Process(out IList<MySoftException> errors)
        {
            //实例化errors
            errors = new List<MySoftException>();
            int rowCount = 0;

            if (commandList.Count == 0)
            {
                //如果命令列表为空，则直接返回
                return rowCount;
            }

            //Access不能进行多任务处理
            if (!dbProvider.SupportBatch)
            {
                foreach (DbCommand cmd in commandList)
                {
                    try
                    {
                        //执行成功，则马上退出
                        rowCount += dbProvider.ExecuteNonQuery(cmd, dbTrans);
                    }
                    catch (DbException ex)
                    {
                        errors.Add(new MySoftException(ex.Message, ex));
                    }

                    //执行一次休眠一下
                    Thread.Sleep(10);
                }
            }
            else
            {
                int size = Convert.ToInt32(Math.Ceiling(commandList.Count * 1.0 / batchSize));
                for (int index = 0; index < size; index++)
                {
                    DbCommand mergeCommand = dbProvider.CreateSqlCommand("init");
                    List<DbCommand> cmdList = new List<DbCommand>();
                    int getSize = batchSize;
                    if ((index + 1) * batchSize > commandList.Count)
                    {
                        getSize = commandList.Count - index * batchSize;
                    }
                    cmdList.AddRange(commandList.GetRange(index * batchSize, getSize));
                    StringBuilder sb = new StringBuilder();

                    int pIndex = 0;
                    foreach (DbCommand cmd in cmdList)
                    {
                        string cmdText = cmd.CommandText;
                        foreach (DbParameter p in cmd.Parameters)
                        {
                            DbParameter newp = (DbParameter)((ICloneable)p).Clone();
                            mergeCommand.Parameters.Add(newp);
                        }
                        sb.Append(cmdText);
                        sb.Append(";\n");

                        pIndex++;
                    }

                    mergeCommand.CommandText = sb.ToString();

                    try
                    {
                        //执行成功，则马上退出
                        rowCount += dbProvider.ExecuteNonQuery(mergeCommand, dbTrans);
                    }
                    catch (DbException ex)
                    {
                        errors.Add(new MySoftException(ex.Message, ex));
                    }

                    //执行一次休眠一下
                    Thread.Sleep(10);
                }
            }

            //结束处理,清除命令列表
            commandList.Clear();

            return rowCount;
        }

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
            List<FieldValue> fvlist = entity.GetFieldValues();
            WhereClip where = null;
            int value = 0;

            if (entity.IsUpdate)
            {
                where = DataUtils.GetPkWhere<T>(entity.GetTable(), entity);
                fvlist.RemoveAll(fv => !fv.IsChanged || fv.IsIdentity || fv.IsPrimaryKey);

                value = Update<T>(table, fvlist, where);
            }
            else
            {
                object retVal;
                fvlist.RemoveAll(fv => fv.IsChanged);

                value = Insert<T>(table, fvlist, out retVal);

                //给标识列赋值
                if (retVal != null)
                {
                    DataUtils.SetPropertyValue(entity, entity.IdentityField.PropertyName, retVal);
                }
            }
            entity.AttachSet();

            return value;
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
            List<FieldValue> fvlist = DataUtils.CreateFieldValue(fields, values, true);
            object retVal;
            return Insert<T>(table, fvlist, out retVal);
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
            List<FieldValue> fvlist = DataUtils.CreateFieldValue(fields, values, true);
            object retValue;
            int ret = Insert<T>(table, fvlist, out retValue);
            retVal = DataUtils.ConvertValue<TResult>(retValue);

            return ret;
        }

        #region 私有方法

        /// <summary>
        /// 添加命令到队列中
        /// </summary>
        /// <param name="cmd"></param>
        private void AddCommand(DbCommand cmd)
        {
            commandList.Add(cmd);
        }

        /// <summary>
        /// 插入值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="fvlist"></param>
        /// <param name="retVal"></param>
        /// <returns></returns>
        private int Insert<T>(Table table, List<FieldValue> fvlist, out object retVal)
            where T : Entity
        {
            int val = 0;
            retVal = null;

            T entity = DataUtils.CreateInstance<T>();
            if (useBatch)
            {
                DbCommand cmd = dbProvider.CreateInsert<T>(table, fvlist, entity.IdentityField, entity.SequenceName);
                AddCommand(cmd);
            }
            else
            {
                val = dbProvider.Insert<T>(table, fvlist, dbTrans, entity.IdentityField, entity.SequenceName, true, out retVal);
            }

            return val;
        }

        /// <summary>
        /// 保存一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="fvlist"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        private int Update<T>(Table table, List<FieldValue> fvlist, WhereClip where)
            where T : Entity
        {
            int val = 0;

            if (useBatch)
            {
                DbCommand cmd = dbProvider.CreateUpdate<T>(table, fvlist, where);
                AddCommand(cmd);
            }
            else
            {
                val = dbProvider.Update<T>(table, fvlist, where, dbTrans);
            }

            return val;
        }

        #endregion

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete<T>(Table table, T entity)
             where T : Entity
        {
            WhereClip where = DataUtils.GetPkWhere<T>(table, entity);
            int val = 0;
            if (useBatch)
            {
                DbCommand cmd = dbProvider.CreateDelete<T>(table, where);
                AddCommand(cmd);
            }
            else
            {
                val = dbProvider.Delete<T>(table, where, dbTrans);
            }
            return val;
        }

        /// <summary>
        /// 按主键删除指定记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public int Delete<T>(Table table, params object[] pkValues)
            where T : Entity
        {
            WhereClip where = DataUtils.GetPkWhere<T>(table, pkValues);
            return dbProvider.Delete<T>(table, where, dbTrans);
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
            if (Exists(table, entity))
                entity.Attach();
            else
                entity.Detach();

            return Save<T>(table, entity);
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
            return Save<T>(null, entity);
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
            return Insert<T>(null, fields, values);
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
            return Insert<T, TResult>(null, fields, values, out retVal);
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
            return Delete<T>(null, entity);
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
            return Delete<T>(null, pkValues);
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
            return InsertOrUpdate(null, entity);
        }

        #endregion

        #endregion

        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool Exists<T>(Table table, T entity)
            where T : Entity
        {
            WhereClip where = DataUtils.GetPkWhere<T>(table, entity);
            FromSection<T> fs = new FromSection<T>(dbProvider, dbTrans, table);
            return fs.Where(where).Count() > 0;
        }
    }
}
