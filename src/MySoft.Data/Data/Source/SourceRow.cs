using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySoft.Data
{
    /// <summary>
    /// 数据行
    /// </summary>
    public class SourceRow : IRowReader
    {
        private DataRow row;

        /// <summary>
        /// 实例化DbRow
        /// </summary>
        /// <param name="row"></param>
        public SourceRow(DataRow row)
        {
            this.row = row;
        }

        /// <summary>
        /// 获取当前DataSource
        /// </summary>
        /// <returns></returns>
        public object DataSource
        {
            get
            {
                return row;
            }
        }

        #region 获取数据

        /// <summary>
        /// 按索引获取值
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object this[int i]
        {
            get
            {
                return this.GetValue(i);
            }
        }

        /// <summary>
        /// 按名称获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                return this.GetValue(name);
            }
        }

        /// <summary>
        /// 判断数据是否为null
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsDBNull(int index)
        {
            if (row == null) return true;
            if (row.Table.Columns.Count - 1 < index) return true;
            return row.IsNull(index);
        }

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetValue(int index)
        {
            object obj = row[index];
            if (obj == DBNull.Value) return null;
            return obj;
        }

        /// <summary>
        /// 返回指定类型的数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public TResult GetValue<TResult>(int index)
        {
            return DataUtils.ConvertValue<TResult>(GetValue(index));
        }

        /// <summary>
        /// 判断数据是否为null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsDBNull(string name)
        {
            if (string.IsNullOrEmpty(name)) return true;
            if (!Contains(name)) return true;
            return row.IsNull(name);
        }

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            object obj = row[name];
            if (obj == DBNull.Value) return null;
            return obj;
        }

        /// <summary>
        /// 返回指定类型的数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public TResult GetValue<TResult>(string name)
        {
            return DataUtils.ConvertValue<TResult>(GetValue(name));
        }

        /// <summary>
        /// 获取string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name)
        {
            return GetValue<string>(name);
        }

        /// <summary>
        /// 获取byte[]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public byte[] GetBytes(string name)
        {
            return GetValue<byte[]>(name);
        }

        /// <summary>
        /// 获取short
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public short GetInt16(string name)
        {
            return GetValue<short>(name);
        }

        /// <summary>
        /// 获取int
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetInt32(string name)
        {
            return GetValue<int>(name);
        }

        /// <summary>
        /// 获取long
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public long GetInt64(string name)
        {
            return GetValue<long>(name);
        }

        /// <summary>
        /// 获取decimal
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public decimal GetDecimal(string name)
        {
            return GetValue<decimal>(name);
        }

        /// <summary>
        /// 获取double
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double GetDouble(string name)
        {
            return GetValue<double>(name);
        }

        /// <summary>
        /// 获取float
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public float GetFloat(string name)
        {
            return GetValue<float>(name);
        }

        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public byte GetByte(string name)
        {
            return GetValue<byte>(name);
        }

        /// <summary>
        /// 获取bool
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetBoolean(string name)
        {
            return GetValue<bool>(name);
        }

        /// <summary>
        /// 获取DateTime
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DateTime GetDateTime(string name)
        {
            return GetValue<DateTime>(name);
        }

        /// <summary>
        /// 获取Guid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Guid GetGuid(string name)
        {
            return GetValue<Guid>(name);
        }

        #region 按字段获取数据

        /// <summary>
        /// 判断数据是否为null
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool IsDBNull(Field field)
        {
            return IsDBNull(field.OriginalName);
        }

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public object GetValue(Field field)
        {
            return GetValue(field.OriginalName);
        }

        /// <summary>
        /// 返回指定类型的数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public TResult GetValue<TResult>(Field field)
        {
            return GetValue<TResult>(field.OriginalName);
        }

        /// <summary>
        /// 获取string
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetString(Field field)
        {
            return GetString(field.OriginalName);
        }

        /// <summary>
        /// 获取byte[]
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public byte[] GetBytes(Field field)
        {
            return GetBytes(field.OriginalName);
        }

        /// <summary>
        /// 获取short
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public short GetInt16(Field field)
        {
            return GetInt16(field.OriginalName);
        }

        /// <summary>
        /// 获取int
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public int GetInt32(Field field)
        {
            return GetInt32(field.OriginalName);
        }

        /// <summary>
        /// 获取long
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public long GetInt64(Field field)
        {
            return GetInt64(field.OriginalName);
        }

        /// <summary>
        /// 获取decimal
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public decimal GetDecimal(Field field)
        {
            return GetDecimal(field.OriginalName);
        }

        /// <summary>
        /// 获取double
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public double GetDouble(Field field)
        {
            return GetDouble(field.OriginalName);
        }

        /// <summary>
        /// 获取float
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public float GetFloat(Field field)
        {
            return GetFloat(field.OriginalName);
        }

        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public byte GetByte(Field field)
        {
            return GetByte(field.OriginalName);
        }

        /// <summary>
        /// 获取bool
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool GetBoolean(Field field)
        {
            return GetBoolean(field.OriginalName);
        }

        /// <summary>
        /// 获取DateTime
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public DateTime GetDateTime(Field field)
        {
            return GetDateTime(field.OriginalName);
        }

        /// <summary>
        /// 获取Guid
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public Guid GetGuid(Field field)
        {
            return GetGuid(field.OriginalName);
        }

        private bool Contains(string name)
        {
            if (row == null) return false;
            return row.Table.Columns.Contains(name);
        }

        #endregion

        // 摘要:
        //     执行与释放或重置非托管资源相关的应用程序定义的任务。
        public void Dispose()
        {
            this.row = null;
        }

        #endregion
    }

}
