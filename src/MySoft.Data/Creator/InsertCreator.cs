using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 插入创建器
    /// </summary>
    [Serializable]
    public class InsertCreator : IInsertCreator
    {
        /// <summary>
        /// 创建一个新的插入器
        /// </summary>
        public static InsertCreator NewCreator()
        {
            return new InsertCreator();
        }

        private Table table;
        private Field identityField;
        private string sequenceName;
        private List<FieldValue> fvlist;

        /// <summary>
        /// 实例化InsertCreator
        /// </summary>
        protected InsertCreator()
        {
            this.fvlist = new List<FieldValue>();
        }

        #region 内部属性

        /// <summary>
        /// 返回table
        /// </summary>
        internal Table Table
        {
            get
            {
                return table;
            }
        }

        /// <summary>
        /// 返回标识列字段
        /// </summary>
        internal Field IdentityField
        {
            get
            {
                return identityField;
            }
        }

        /// <summary>
        /// 返回自增量名称
        /// </summary>
        internal string SequenceName
        {
            get
            {
                return sequenceName;
            }
        }

        internal List<FieldValue> FieldValues
        {
            get
            {
                return fvlist;
            }
        }

        #endregion

        #region 设置表信息

        /// <summary>
        /// 设置表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public InsertCreator From<T>()
            where T : Entity
        {
            this.table = Table.GetTable<T>();
            return this;
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        public InsertCreator From(string tableName)
        {
            this.table = new Table(tableName);
            return this;
        }

        /// <summary>
        /// 设置表信息
        /// </summary>
        /// <param name="table"></param>
        public InsertCreator From(Table table)
        {
            this.table = table;
            return this;
        }

        /// <summary>
        /// 设置实体信息
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public InsertCreator SetEntity<T>(T entity)
            where T : Entity
        {
            //获取需要插入的值
            this.fvlist = entity.GetFieldValues();
            this.fvlist.RemoveAll(fv => fv.IsChanged);

            this.sequenceName = entity.SequenceName;

            this.fvlist.ForEach(fv =>
            {
                if (fv.IsIdentity)
                {
                    this.identityField = fv.Field;
                }
            });

            return this.From<T>();
        }

        #endregion

        #region 设置主键字段

        /// <summary>
        /// 设置主键字段
        /// </summary>
        /// <param name="field"></param>
        public InsertCreator SetIdentityField(Field field)
        {
            this.identityField = field;
            return this;
        }

        /// <summary>
        /// 设置主键字段
        /// </summary>
        /// <param name="fieldName"></param>
        public InsertCreator SetIdentityField(string fieldName)
        {
            return SetIdentityField(new Field(fieldName));
        }

        #endregion

        #region 添加字段和值

        /// <summary>
        /// 添加一个数据
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public InsertCreator AddInsert(Field field, object value)
        {
            //存在相同字段，则不加入插入列表
            if (!fvlist.Exists(fv => { return fv.Field.OriginalName == field.OriginalName; }))
            {
                FieldValue fv = new FieldValue(field, value);
                fvlist.Add(fv);
            }

            return this;
        }

        /// <summary>
        /// 添加一个数据
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public InsertCreator AddInsert(string fieldName, object value)
        {
            return AddInsert(new Field(fieldName), value);
        }

        /// <summary>
        /// 添加一个数据字典
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public InsertCreator AddInsert(IDictionary<string, object> dict)
        {
            string[] fields = new List<string>(dict.Keys).ToArray();
            object[] values = new List<object>(dict.Values).ToArray();
            return AddInsert(fields, values);
        }

        /// <summary>
        /// 添加一个数据字典
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public InsertCreator AddInsert(IDictionary<Field, object> dict)
        {
            Field[] fields = new List<Field>(dict.Keys).ToArray();
            object[] values = new List<object>(dict.Values).ToArray();
            return AddInsert(fields, values);
        }

        /// <summary>
        /// 添加多个数据
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        public InsertCreator AddInsert(Field[] fields, object[] values)
        {
            if (fields == null || values == null)
            {
                throw new MySoftException("字段和值不能为null;");
            }

            if (fields.Length != values.Length)
            {
                throw new MySoftException("字段和值的数量必须一致;");
            }

            int index = 0;
            foreach (Field field in fields)
            {
                AddInsert(field, values[index]);
                index++;
            }

            return this;
        }

        /// <summary>
        /// 添加多个数据
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <param name="values"></param>
        public InsertCreator AddInsert(string[] fieldNames, object[] values)
        {
            if (fieldNames == null || values == null)
            {
                throw new MySoftException("字段和值不能为null;");
            }

            if (fieldNames.Length != values.Length)
            {
                throw new MySoftException("字段和值的数量必须一致;");
            }

            int index = 0;
            foreach (string fieldName in fieldNames)
            {
                AddInsert(fieldName, values[index]);
                index++;
            }

            return this;
        }

        /// <summary>
        /// 移除指定的列
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public InsertCreator RemoveInsert(params string[] fieldNames)
        {
            if (fieldNames == null) return this;

            List<Field> fields = new List<Field>();
            foreach (string fieldName in fieldNames)
            {
                fields.Add(new Field(fieldName));
            }

            return RemoveInsert(fields.ToArray());
        }

        /// <summary>
        /// 移除指定的列
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public InsertCreator RemoveInsert(params Field[] fields)
        {
            if (fields == null) return this;

            foreach (Field field in fields)
            {
                int count = this.fvlist.RemoveAll(fv =>
                {
                    return string.Compare(fv.Field.OriginalName, field.OriginalName, true) == 0;
                });

                if (count == 0)
                {
                    throw new MySoftException("指定的字段不存在于Insert列表中！");
                }
            }

            return this;
        }

        #endregion
    }
}
