using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 行数据阅读器
    /// </summary>
    public interface IRowReader : IDisposable
    {
        /// <summary>
        /// 获取当前DataSource
        /// </summary>
        /// <returns></returns>
        object DataSource { get; }

        #region 读取数据

        /// <summary>
        /// 按索引获取值
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        object this[int i] { get; }

        /// <summary>
        /// 按名称获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object this[string name] { get; }

        #region 接索引获取数据

        /// <summary>
        /// 判断数据是否为null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsDBNull(int index);

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object GetValue(int index);

        /// <summary>
        /// 返回指定类型的数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        TResult GetValue<TResult>(int index);

        #endregion

        #region 获取数据

        /// <summary>
        /// 判断数据是否为null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsDBNull(string name);

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetValue(string name);

        /// <summary>
        /// 返回指定类型的数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        TResult GetValue<TResult>(string name);

        /// <summary>
        /// 获取string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetString(string name);

        /// <summary>
        /// 获取byte[]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        byte[] GetBytes(string name);

        /// <summary>
        /// 获取short
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        short GetInt16(string name);

        /// <summary>
        /// 获取int
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetInt32(string name);

        /// <summary>
        /// 获取long
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        long GetInt64(string name);

        /// <summary>
        /// 获取decimal
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        decimal GetDecimal(string name);

        /// <summary>
        /// 获取double
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double GetDouble(string name);

        /// <summary>
        /// 获取float
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        float GetFloat(string name);

        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        byte GetByte(string name);

        /// <summary>
        /// 获取bool
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetBoolean(string name);

        /// <summary>
        /// 获取DateTime
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        DateTime GetDateTime(string name);

        /// <summary>
        /// 获取Guid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Guid GetGuid(string name);

        #endregion

        #region 按字段获取数据

        /// <summary>
        /// 判断数据是否为null
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        bool IsDBNull(Field field);

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        object GetValue(Field field);

        /// <summary>
        /// 返回指定类型的数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        TResult GetValue<TResult>(Field field);

        /// <summary>
        /// 获取string
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        string GetString(Field field);

        /// <summary>
        /// 获取byte[]
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        byte[] GetBytes(Field field);

        /// <summary>
        /// 获取short
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        short GetInt16(Field field);

        /// <summary>
        /// 获取int
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        int GetInt32(Field field);

        /// <summary>
        /// 获取long
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        long GetInt64(Field field);

        /// <summary>
        /// 获取decimal
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        decimal GetDecimal(Field field);

        /// <summary>
        /// 获取double
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        double GetDouble(Field field);

        /// <summary>
        /// 获取float
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        float GetFloat(Field field);

        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        byte GetByte(Field field);

        /// <summary>
        /// 获取bool
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        bool GetBoolean(Field field);

        /// <summary>
        /// 获取DateTime
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        DateTime GetDateTime(Field field);

        /// <summary>
        /// 获取Guid
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        Guid GetGuid(Field field);

        #endregion

        #endregion
    }
}
