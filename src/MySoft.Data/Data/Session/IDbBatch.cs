using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace MySoft.Data
{
    interface IDbProcess
    {
        #region 增删改操作

        int Insert<T>(Field[] fields, object[] values) where T : Entity;
        int Insert<T, TResult>(Field[] fields, object[] values, out TResult retVal) where T : Entity;

        int Save<T>(T entity) where T : Entity;
        int Delete<T>(T entity) where T : Entity;
        int Delete<T>(params object[] pkValues) where T : Entity;

        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        int InsertOrUpdate<T>(T entity) where T : Entity;

        #endregion

        #region 增删改操作(分表处理)

        int Insert<T>(Table table, Field[] fields, object[] values) where T : Entity;
        int Insert<T, TResult>(Table table, Field[] fields, object[] values, out TResult retVal) where T : Entity;

        int Save<T>(Table table, T entity) where T : Entity;
        int Delete<T>(Table table, T entity) where T : Entity;
        int Delete<T>(Table table, params object[] pkValues) where T : Entity;

        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        int InsertOrUpdate<T>(Table table, T entity) where T : Entity;

        #endregion
    }

    interface IDbBatch : IDbProcess
    {
        /// <summary>
        /// 执行批处理操作
        /// </summary>
        int Process();

        /// <summary>
        /// 执行批处理操作
        /// </summary>
        /// <param name="errors">输出的错误</param>
        /// <returns></returns>
        int Process(out IList<MySoftException> errors);
    }
}
