using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
using MySoft.Data.Design;

namespace MySoft.Data
{
    /// <summary>
    /// 自定义数据记录
    /// </summary>
    public sealed class SourceReader : ISourceReader
    {
        private IDataReader reader;
        private IDictionary<string, int> dictIndex;

        /// <summary>
        /// 初始化DbReader
        /// </summary>
        /// <param name="reader"></param>
        public SourceReader(IDataReader reader)
        {
            this.reader = reader;
            this.dictIndex = new Dictionary<string, int>();

            if (!reader.IsClosed)
            {
                //把reader中存在的字段加入列表中
                for (int index = 0; index < reader.FieldCount; index++)
                {
                    string name = reader.GetName(index);
                    name = name.ToLower();
                    this.dictIndex.Add(name, index);
                }
            }
        }

        /// <summary>
        /// 获取字段数
        /// </summary>
        public int FieldCount
        {
            get
            {
                return reader.FieldCount;
            }
        }

        /// <summary>
        /// 获取当前DataSource
        /// </summary>
        /// <returns></returns>
        public object DataSource
        {
            get
            {
                return reader;
            }
        }

        /// <summary>
        /// 读取下一条数据
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            return reader.Read();
        }

        /// <summary>
        /// 返回下一结果集
        /// </summary>
        /// <returns></returns>
        public bool NextResult()
        {
            return reader.NextResult();
        }

        /// <summary>
        /// 关闭DbReader
        /// </summary>
        public void Close()
        {
            reader.Close();
        }

        // 摘要:
        //     执行与释放或重置非托管资源相关的应用程序定义的任务。
        public void Dispose()
        {
            reader.Dispose();
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
            return reader.IsDBNull(index);
        }

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetValue(int index)
        {
            object obj = reader.GetValue(index);
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
            if (reader.IsClosed) return true;

            name = name.ToLower();
            if (!dictIndex.ContainsKey(name)) return true;
            return reader.IsDBNull(dictIndex[name]);
        }

        /// <summary>
        /// 返回object
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (reader.IsClosed) return null;

            name = name.ToLower();
            if (!dictIndex.ContainsKey(name)) return null;
            return GetValue(dictIndex[name]);
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

        #endregion

        #endregion

        /// <summary>
        /// 返回指定类型的List
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public SourceList<TOutput> ConvertTo<TOutput>()
        {
            return this.ConvertAll<TOutput>(p => DataUtils.ConvertType<IRowReader, TOutput>(p));
        }

        /// <summary>
        /// 返回另一类型的列表(输入为类、输出为接口，用于实体的解耦)
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="IOutput"></typeparam>
        /// <returns></returns>
        public SourceList<IOutput> ConvertTo<TOutput, IOutput>()
            where TOutput : IOutput
        {
            if (!typeof(TOutput).IsClass)
            {
                throw new MySoftException("TOutput必须是Class类型！");
            }

            if (!typeof(IOutput).IsInterface)
            {
                throw new MySoftException("IOutput必须是Interface类型！");
            }

            //进行两次转换后返回
            return ConvertTo<TOutput>().ConvertTo<IOutput>();
        }

        /// <summary>
        /// 通过委托来创建实体
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SourceList<TOutput> ConvertAll<TOutput>(Converter<IRowReader, TOutput> handler)
        {
            SourceList<TOutput> list = new SourceList<TOutput>();
            while (this.Read())
            {
                //读取数据到实体
                list.Add(handler(this));
            }
            this.Close();
            this.Dispose();

            return list;
        }
    }
}
