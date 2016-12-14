using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    interface IDbSession : IDbTrans
    {
        #region Trans操作

        void SetProvider(string connectName);
        void SetProvider(DbProvider dbProvider);

        DbTrans BeginTrans();
        DbTrans BeginTrans(IsolationLevel isolationLevel);
        DbTrans SetTransaction(DbTransaction trans);
        DbTrans SetConnection(DbConnection connection);
        DbTransaction BeginTransaction();
        DbTransaction BeginTransaction(IsolationLevel isolationLevel);
        DbConnection CreateConnection();
        DbParameter CreateParameter();
        #endregion

        #region 注册Log

        void RegisterSqlLogger(LogHandler handler);
        void UnregisterSqlLogger(LogHandler handler);

        void RegisterSqlExceptionLogger(ExceptionLogHandler handler);
        void UnregisterSqlExceptionLogger(ExceptionLogHandler handler);

        #endregion

        #region 缓存操作

        void CacheOn();
        void CacheOff();
        void RemoveCache<T>();
        void RemoveAllCache();

        #endregion

        #region 系列化操作

        string Serialization(WhereClip where);
        string Serialization(OrderByClip order);

        #endregion
    }
}
