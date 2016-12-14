using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 表信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Table<T> : Table
        where T : class
    {
        /// <summary>
        /// 实例化一个表
        /// </summary>
        /// <param name="tableName"></param>
        public Table(string tableName)
            : base(tableName)
        {
            Table table = EntityConfig.Instance.GetMappingTable<T>(tableName);
            this.TableName = table.TableName;
            this.Prefix = table.Prefix;
            this.Suffix = table.Suffix;
        }
    }

    /// <summary>
    /// 数据库查询时用户传入的自定义信息表
    /// </summary>
    [Serializable]
    public class Table : ITable
    {
        private static readonly IDictionary<Type, Table> dictTable = new Dictionary<Type, Table>();

        /// <summary>
        /// 实例化一个表
        /// </summary>
        /// <param name="tableName"></param>
        public Table(string tableName)
        {
            this.name = tableName.Replace("{0}", "").Replace("{1}", "");
            this.prefix = null;
            this.suffix = null;
        }

        /// <summary>
        /// 指定表别名
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public Table As(string aliasName)
        {
            this.aliasName = aliasName;
            return this;
        }

        /// <summary>
        /// 返回一个Field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public Field this[string fieldName]
        {
            get
            {
                return new Field(fieldName).At(this);
            }
        }

        /// <summary>
        /// 表名称
        /// </summary>
        internal string Name
        {
            get
            {
                return string.Concat("{0}", OriginalName, "{1}");
            }
        }

        private string name;
        /// <summary>
        /// 设置表名
        /// </summary>
        internal string TableName
        {
            get { return name; }
            set { name = value; }
        }

        private string aliasName;
        /// <summary>
        /// 表别名
        /// </summary>
        internal string AliasName
        {
            get { return aliasName; }
        }

        private string prefix;
        /// <summary>
        /// 设置表前缀
        /// </summary>
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        private string suffix;
        /// <summary>
        /// 设置表后缀
        /// </summary>
        public string Suffix
        {
            get { return suffix; }
            set { suffix = value; }
        }

        /// <summary>
        /// 获取原始的表名
        /// </summary>
        public string OriginalName
        {
            get
            {
                return string.Format("{0}{1}{2}", prefix, name, suffix);
            }
        }

        /// <summary>
        /// 返回一个Table实例
        /// </summary>
        /// <returns></returns>
        public static Table GetTable<T>()
            where T : Entity
        {
            lock (dictTable)
            {
                if (dictTable.ContainsKey(typeof(T)))
                {
                    return dictTable[typeof(T)];
                }
                else
                {
                    Table table = DataUtils.CreateInstance<T>().GetTable();
                    dictTable.Add(typeof(T), table);

                    return table;
                }
            }
        }

        /// <summary>
        /// 返回一个Table实例
        /// </summary>
        /// <param name="suffix">后缀名称</param>
        /// <returns></returns>
        public static Table GetTable<T>(string suffix)
            where T : Entity
        {
            Table table = GetTable<T>();
            table.Suffix = suffix;

            return table;
        }

        /// <summary>
        /// 返回一个Table实例
        /// </summary>
        /// <param name="prefix">前缀名称</param>
        /// <param name="suffix">后缀名称</param>
        /// <returns></returns>
        public static Table GetTable<T>(string prefix, string suffix)
            where T : Entity
        {
            Table table = GetTable<T>();
            table.Prefix = prefix;
            table.Suffix = suffix;

            return table;
        }

        /// <summary>
        /// 返回一个表关系
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TableRelation<T> From<T>()
            where T : Entity
        {
            return From<T>(null);
        }

        /// <summary>
        /// 返回一个表关系
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TableRelation<T> From<T>(Table table)
            where T : Entity
        {
            return new TableRelation<T>(table);
        }
    }
}
