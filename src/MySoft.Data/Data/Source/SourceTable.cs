using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Data;
using System.Reflection;
using MySoft.Data.Design;

namespace MySoft.Data
{
    /// <summary>
    /// 获取返回值的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate object ReturnValue<T>(T value);

    /// <summary>
    /// 表信息填充关系
    /// </summary>
    [Serializable]
    public class FillRelation
    {
        /// <summary>
        /// 填充用到的数据源
        /// </summary>
        public SourceTable DataSource { get; set; }

        /// <summary>
        /// 关联的数据列
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 关联的数据列
        /// </summary>
        public string ChildName { get; set; }

        /// <summary>
        /// 实例化填充关系
        /// </summary>
        public FillRelation() { }

        /// <summary>
        /// 实例化填充关系
        /// </summary>
        /// <param name="source"></param>
        /// <param name="relationName"></param>
        public FillRelation(SourceTable source, string relationName)
            : this(source, relationName, relationName)
        { }

        /// <summary>
        /// 实例化填充关系
        /// </summary>
        /// <param name="source"></param>
        /// <param name="relationName"></param>
        public FillRelation(SourceTable source, string parentName, string childName)
        {
            this.DataSource = source;
            this.ParentName = parentName;
            this.ChildName = childName;
        }
    }

    /// <summary>
    /// 数据源
    /// </summary>
    [Serializable]
    public class SourceTable : DataTable, ISourceTable
    {
        /// <summary>
        /// 实例化SourceTable
        /// </summary>
        public SourceTable()
        {
            this.TableName = "NewTable";
        }

        /// <summary>
        /// 实例化SourceTable
        /// </summary>
        public SourceTable(string tableName)
        {
            base.TableName = tableName;
        }

        /// <summary>
        /// 实例化SourceTable
        /// </summary>
        /// <param name="dt"></param>
        public SourceTable(DataTable dt)
            : this()
        {
            if (dt != null)
            {
                if (!string.IsNullOrEmpty(dt.TableName))
                    this.TableName = dt.TableName;

                foreach (DataColumn column in dt.Columns)
                {
                    this.Columns.Add(column.ColumnName, column.DataType);
                }
                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        this.ImportRow(row);
                    }
                }
            }
        }

        protected SourceTable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }


        /// <summary>
        /// 获取数据行数
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.Rows.Count;
            }
        }

        /// <summary>
        /// 获取数据列数
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return this.Columns.Count;
            }
        }

        /// <summary>
        /// 获取行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IRowReader this[int index]
        {
            get
            {
                DataRow row = base.Rows[index];
                return new SourceRow(row);
            }
        }

        /// <summary>
        /// 克隆Table
        /// </summary>
        /// <returns></returns>
        public new SourceTable Clone()
        {
            DataTable table = base.Copy();
            return new SourceTable(table);
        }

        /// <summary>
        /// 选择某些列
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public SourceTable Select(params string[] names)
        {
            SourceTable st = this.Clone();
            List<string> namelist = new List<string>(names);
            namelist.ForEach(p => p.ToLower());
            foreach (DataColumn column in this.Columns)
            {
                if (!namelist.Contains(column.ColumnName.ToLower()))
                    st.Columns.Remove(column.ColumnName);
            }

            int index = 0;
            namelist.ForEach(p => st.Columns[p].SetOrdinal(index++));

            return st;
        }

        /// <summary>
        /// 过虑数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SourceTable Filter(string expression)
        {
            DataRow[] rows = base.Select(expression);
            DataTable dt = base.Clone();
            if (rows != null && rows.Length > 0)
            {
                foreach (DataRow row in rows)
                {
                    dt.ImportRow(row);
                }
            }
            return new SourceTable(dt);
        }

        /// <summary>
        /// 排序数据
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public SourceTable Sort(string sort)
        {
            DataRow[] rows = base.Select(null, sort);
            DataTable dt = base.Clone();
            if (rows != null && rows.Length > 0)
            {
                foreach (DataRow row in rows)
                {
                    dt.ImportRow(row);
                }
            }
            return new SourceTable(dt);
        }

        /// <summary>
        /// 移除指定的列
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public void Remove(params string[] names)
        {
            List<string> namelist = new List<string>(names);
            namelist.ForEach(p =>
            {
                if (this.Columns.Contains(p))
                    this.Columns.Remove(p);
            });
        }

        /// <summary>
        /// 设置列的顺序
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        public void SetOrdinal(string name, int index)
        {
            if (!this.Columns.Contains(name))
                throw new MySoftException(string.Format("当前表中不存在字段【{0}】！", name));

            this.Columns[name].SetOrdinal(index);
        }

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public void Add(string name, Type type)
        {
            this.Columns.Add(name, type);
        }

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public void Add(string name, Type type, string expression)
        {
            this.Columns.Add(name, type, expression);
        }

        /// <summary>
        /// 字段更名
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        /// <returns></returns>
        public void Rename(string oldname, string newname)
        {
            if (!this.Columns.Contains(oldname))
                throw new MySoftException(string.Format("当前表中不存在字段【{0}】！", oldname));

            if (string.IsNullOrEmpty(newname))
                throw new MySoftException("设置的新列名不能为null或空！");

            if (this.Columns.Contains(newname))
                throw new MySoftException(string.Format("当前表中已存在字段【{0}】！", newname));

            DataColumn column = this.Columns[oldname];
            column.ColumnName = newname;
        }

        /// <summary>
        /// 按要求改变某列值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="readName"></param>
        /// <param name="changeName"></param>
        /// <param name="revalue"></param>
        public void Revalue<T>(string readName, string changeName, ReturnValue<T> revalue)
        {
            if (!this.Columns.Contains(readName))
                throw new MySoftException(string.Format("当前表中不存在字段【{0}】！", readName));

            if (!this.Columns.Contains(changeName))
                throw new MySoftException(string.Format("当前表中不存在字段【{0}】！", changeName));

            //按要求改变值
            foreach (DataRow row in this.Rows)
            {
                row[changeName] = revalue(DataUtils.ConvertValue<T>(row[readName]));
            }
        }

        /// <summary>
        /// 将另一个表中的某字段值按字段关联后进行填充
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="fillNames"></param>
        public void Fill(FillRelation relation, params string[] fillNames)
        {
            var source = relation.DataSource;
            var parentName = relation.ParentName;
            var childName = relation.ChildName;

            if (relation == null) return;
            if (source == null || source.RowCount == 0) return;
            if (fillNames == null || fillNames.Length == 0) return;

            if (!this.Columns.Contains(parentName))
            {
                throw new MySoftException(string.Format("当前表中不存在字段【{0}】！", parentName));
            }
            if (!source.Columns.Contains(childName))
            {
                throw new MySoftException(string.Format("关联表中不存在字段【{0}】！", childName));
            }

            //为当前Table添加上相应的列
            for (int i = 0; i < fillNames.Length; i++)
            {
                //判断表中是否存在字段
                if (!source.Columns.Contains(fillNames[i]))
                {
                    throw new MySoftException(string.Format("关联表中不存在字段【{0}】！", fillNames[i]));
                }

                if (!this.Columns.Contains(fillNames[i]))
                    this.Columns.Add(fillNames[i], source.Columns[fillNames[i]].DataType);
            }

            //取出对应的值存入字典
            IDictionary<string, IRowReader> _dictValue = new Dictionary<string, IRowReader>();
            for (int index = 0; index < source.RowCount; index++)
            {
                IRowReader reader = source[index];
                string codeKey = reader.GetString(childName);
                _dictValue[codeKey] = reader;
            }

            //对当前表进行数据填充
            foreach (DataRow row in this.Rows)
            {
                string codeKey = row[parentName].ToString();
                if (!_dictValue.ContainsKey(codeKey)) continue;
                IRowReader reader = _dictValue[codeKey];
                for (int i = 0; i < fillNames.Length; i++)
                {
                    if (source.Columns.Contains(fillNames[i]))
                    {
                        object value = reader[fillNames[i]];
                        row[fillNames[i]] = value == null ? DBNull.Value : value;
                    }
                }
            }
        }

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
        /// 返回指定类型的List
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public SourceList<TOutput> ConvertAll<TOutput>(Converter<IRowReader, TOutput> handler)
        {
            SourceList<TOutput> list = new SourceList<TOutput>();
            for (int index = 0; index < this.RowCount; index++)
            {
                list.Add(handler(this[index]));
            }
            return list;
        }

        // 摘要:
        //     执行与释放或重置非托管资源相关的应用程序定义的任务。
        public new void Dispose()
        {
            this.Rows.Clear();
            this.Columns.Clear();
            base.Dispose(true);
        }
    }
}
