using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using MySoft.Data.Design;

namespace MySoft.Data
{
    /// <summary>
    /// 用于实体内的Field操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Field<T> : Field
        where T : Entity
    {
        public Field(string fieldName)
            : this(fieldName, fieldName)
        { }

        public Field(string propertyName, string fieldName)
            : base(propertyName, null, fieldName)
        {
            this.tableName = Table.GetTable<T>().OriginalName;

            Field field = EntityConfig.Instance.GetMappingField<T>(propertyName, fieldName);
            this.fieldName = field.OriginalName;
        }
    }

    /// <summary>
    /// 用于实体内的Field操作
    /// </summary>
    [Serializable]
    public class Field : IField
    {
        /// <summary>
        /// 字段*
        /// </summary>
        public static readonly AllField All = new AllField();
        protected string propertyName;
        protected string tableName;
        protected string fieldName;
        protected string aliasName;

        #region 内联字段

        internal string FullName
        {
            get
            {
                if (aliasName == null)
                {
                    return this.Name;
                }
                return this.Name + " as {0}" + aliasName + "{1}";
            }
        }

        /// <summary>
        /// 获取真实字段名
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (tableName == null)
                {
                    return FieldName;
                }
                return TableName + "." + FieldName;
            }
        }

        /// <summary>
        /// 获取原始的字段名
        /// </summary>
        public string OriginalName
        {
            get
            {
                if (aliasName != null)
                {
                    return aliasName;
                }
                return fieldName;
            }
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName
        {
            get
            {
                return propertyName;
            }
        }

        #region 私有成员

        private string TableName
        {
            get
            {
                if (tableName == null)
                {
                    return tableName;
                }
                return string.Concat("{0}", tableName.Replace("{0}", "").Replace("{1}", ""), "{1}");
            }
        }

        private string FieldName
        {
            get
            {
                if (fieldName == "*" || fieldName.Contains("'") || fieldName.Contains("(") || fieldName.Contains(")") || fieldName.Contains("{0}") || fieldName.Contains("{1}"))
                {
                    return fieldName;
                }
                return "{0}" + fieldName + "{1}";
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 实例化Field
        /// </summary>
        /// <param name="fieldName"></param>
        public Field(string fieldName)
        {
            this.propertyName = fieldName;
            this.fieldName = fieldName;
        }

        /// <summary>
        /// 实例化Field
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        internal Field(string tableName, string fieldName)
            : this(fieldName)
        {
            this.tableName = string.IsNullOrEmpty(tableName) ? null : tableName;
        }

        /// <summary>
        /// 实例化Field
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        internal Field(string propertyName, string tableName, string fieldName)
            : this(tableName, fieldName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// 实例化Field
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="aliasName"></param>
        internal Field(string propertyName, string tableName, string fieldName, string aliasName)
            : this(propertyName, tableName, fieldName)
        {
            this.aliasName = string.IsNullOrEmpty(aliasName) ? null : aliasName;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region 排序操作

        public OrderByClip Asc
        {
            get
            {
                return new OrderByClip(this.Name + " asc ");
            }
        }

        public OrderByClip Desc
        {
            get
            {
                return new OrderByClip(this.Name + " desc ");
            }
        }

        public GroupByClip Group
        {
            get
            {
                return new GroupByClip(this.Name);
            }
        }

        #endregion

        #region 返回条件

        #region 操作符重载

        public static WhereClip operator ==(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new WhereClip(leftField.Name + " = " + rightField.Name);
        }

        public static WhereClip operator !=(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new WhereClip(leftField.Name + " <> " + rightField.Name);
        }

        public static WhereClip operator >(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new WhereClip(leftField.Name + " > " + rightField.Name);
        }

        public static WhereClip operator >=(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new WhereClip(leftField.Name + " >= " + rightField.Name);
        }

        public static WhereClip operator <(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new WhereClip(leftField.Name + " < " + rightField.Name);
        }

        public static WhereClip operator <=(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new WhereClip(leftField.Name + " <= " + rightField.Name);
        }

        public static WhereClip operator ==(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return CreateWhereClip(field, "=", value);
        }

        public static WhereClip operator !=(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return CreateWhereClip(field, "<>", value);
        }

        public static WhereClip operator >(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return CreateWhereClip(field, ">", value);
        }

        public static WhereClip operator >=(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return CreateWhereClip(field, ">=", value);
        }

        public static WhereClip operator <(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return CreateWhereClip(field, "<", value);
        }

        public static WhereClip operator <=(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return CreateWhereClip(field, "<=", value);
        }

        #endregion

        #region 返回字段

        #region 操作符重载

        public static Field operator +(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new Field(leftField.Name + " + " + rightField.Name).As(leftField.OriginalName);
        }

        public static Field operator -(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new Field(leftField.Name + " - " + rightField.Name).As(leftField.OriginalName);
        }

        public static Field operator *(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new Field(leftField.Name + " * " + rightField.Name).As(leftField.OriginalName);
        }

        public static Field operator /(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new Field(leftField.Name + " / " + rightField.Name).As(leftField.OriginalName);
        }

        public static Field operator %(Field leftField, Field rightField)
        {
            if ((IField)leftField == null)
            {
                return null;
            }
            if ((IField)rightField == null)
            {
                return null;
            }
            return new Field(leftField.Name + " % " + rightField.Name).As(leftField.OriginalName);
        }

        public static Field operator +(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(field.Name + " + " + DataUtils.FormatValue(value)).As(field.OriginalName);
        }

        public static Field operator -(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(field.Name + " - " + DataUtils.FormatValue(value)).As(field.OriginalName);
        }

        public static Field operator *(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(field.Name + " * " + DataUtils.FormatValue(value)).As(field.OriginalName);
        }

        public static Field operator /(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(field.Name + " / " + DataUtils.FormatValue(value)).As(field.OriginalName);
        }

        public static Field operator %(Field field, object value)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(field.Name + " % " + DataUtils.FormatValue(value)).As(field.OriginalName);
        }

        public static Field operator +(object value, Field field)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(DataUtils.FormatValue(value) + " + " + field.Name).As(field.OriginalName);
        }

        public static Field operator -(object value, Field field)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(DataUtils.FormatValue(value) + " - " + field.Name).As(field.OriginalName);
        }

        public static Field operator *(object value, Field field)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(DataUtils.FormatValue(value) + " * " + field.Name).As(field.OriginalName);
        }

        public static Field operator /(object value, Field field)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(DataUtils.FormatValue(value) + " / " + field.Name).As(field.OriginalName);
        }

        public static Field operator %(object value, Field field)
        {
            if ((IField)field == null)
            {
                return null;
            }
            return new Field(DataUtils.FormatValue(value) + " % " + field.Name).As(field.OriginalName);
        }

        #endregion

        #region 字段操作

        /// <summary>
        /// 对字段进行Distinct操作
        /// </summary>
        /// <returns></returns>
        public Field Distinct()
        {
            return new Field("distinct(" + this.Name + ")");
        }

        /// <summary>
        /// 对字段进行Count操作
        /// </summary>
        /// <returns></returns>
        public Field Count()
        {
            return new Field("count(" + this.Name + ")").As(this.OriginalName);
        }

        /// <summary>
        /// 对字段进行Sum操作
        /// </summary>
        /// <returns></returns>
        public Field Sum()
        {
            return new Field("sum(" + this.Name + ")").As(this.OriginalName);
        }

        /// <summary>
        /// 对字段进行Avg操作
        /// </summary>
        /// <returns></returns>
        public Field Avg()
        {
            return new Field("avg(" + this.Name + ")").As(this.OriginalName);
        }

        /// <summary>
        /// 对字段进行Max操作
        /// </summary>
        /// <returns></returns>
        public Field Max()
        {
            return new Field("max(" + this.Name + ")").As(this.OriginalName);
        }

        /// <summary>
        /// 对字段进行Min操作
        /// </summary>
        /// <returns></returns>
        public Field Min()
        {
            return new Field("min(" + this.Name + ")").As(this.OriginalName);
        }

        /// <summary>
        /// 更改字段的别名
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public Field As(string aliasName)
        {
            return new Field(this.propertyName, this.tableName, this.fieldName, aliasName);
        }

        /// <summary>
        /// 更改字段所在表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Field At(string tableName)
        {
            return new Field(this.propertyName, tableName, this.fieldName);
        }

        /// <summary>
        /// 更改字段所在表
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public Field At(Table table)
        {
            if (table == null) return this;
            return new Field(this.propertyName, table.Name, this.fieldName);
        }

        #endregion

        #endregion

        /// <summary>
        /// 返回一个值为null的条件
        /// </summary>
        /// <returns></returns>
        public WhereClip IsNull()
        {
            return this == (object)null;
        }

        /// <summary>
        /// 指定value进行模糊查询
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereClip Contains(string value)
        {
            return Like("%" + value + "%");
        }

        /// <summary>
        /// 指定value进行Like查询
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereClip Like(string value)
        {
            return CreateWhereClip(this, "like", value);
        }

        /// <summary>
        /// 进行Between操作
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public WhereClip Between(object leftValue, object rightValue)
        {
            return this >= leftValue && this <= rightValue;
        }

        /// <summary>
        /// 进行In子查询操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public WhereClip In<T>(Field field)
            where T : Entity
        {
            return In<T>(null, field);
        }

        /// <summary>
        /// 进行In子查询操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public WhereClip In<T>(Table table, Field field)
            where T : Entity
        {
            return In<T>(table, field, WhereClip.All);
        }

        /// <summary>
        /// 进行In子查询操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereClip In<T>(Field field, WhereClip where)
            where T : Entity
        {
            return In<T>(null, field, where);
        }

        /// <summary>
        /// 进行In子查询操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereClip In<T>(Table table, Field field, WhereClip where)
            where T : Entity
        {
            return In<T>(new FromSection<T>(table).Select(field).Where(where));
        }

        /// <summary>
        /// 进行In操作,query为一个子查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public WhereClip In<T>(QuerySection<T> query)
            where T : Entity
        {
            return new WhereClip(this.Name + " in (" + query.QueryString + ") ", query.Parameters);
        }

        /// <summary>
        /// 进行In操作,query为一个子查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top"></param>
        /// <returns></returns>
        public WhereClip In<T>(TopSection<T> top)
            where T : Entity
        {
            return new WhereClip(this.Name + " in (" + top.QueryString + ") ", top.Parameters);
        }

        /// <summary>
        /// 进行In操作,relation为一个关联查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relation"></param>
        /// <returns></returns>
        public WhereClip In<T>(TableRelation<T> relation)
            where T : Entity
        {
            QuerySection<T> q = relation.Section.Query;
            return new WhereClip(this.Name + " in (" + q.QueryString + ") ", q.Parameters);
        }

        /// <summary>
        /// 进行In子查询操作
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public WhereClip In(params object[] values)
        {
            values = DataUtils.CheckAndReturnValues(values);

            //如果值只有一个的时候，直接使用相等处理
            if (values.Length == 1)
            {
                return this == values[0];
            }
            else
            {
                List<SQLParameter> plist = new List<SQLParameter>();
                StringBuilder sb = new StringBuilder();
                foreach (object value in values)
                {
                    string pName = DataUtils.MakeUniqueKey(30, "$p");
                    SQLParameter p = new SQLParameter(pName);
                    p.Value = value;

                    sb.Append(pName);
                    sb.Append(",");

                    plist.Add(p);
                }

                string where = sb.Remove(sb.Length - 1, 1).ToString().Trim();

                return new WhereClip(this.Name + " in (" + where + ") ", plist.ToArray());
            }
        }

        /// <summary>
        /// 进行In子查询操作
        /// </summary>
        /// <param name="creator"></param>
        /// <returns></returns>
        public WhereClip In(QueryCreator creator)
        {
            QuerySection<TempTable> query = GetQuery(creator);
            return In<TempTable>(query);
        }

        #endregion

        #region 私有方法

        internal QuerySection<TempTable> GetQuery(QueryCreator creator)
        {
            if (creator.Table == null)
            {
                throw new MySoftException("用创建器操作时，表不能为null！");
            }

            FromSection<TempTable> f = new FromSection<TempTable>(creator.Table);
            if (creator.IsRelation)
            {
                foreach (TableJoin join in creator.Relations.Values)
                {
                    if (join.Type == JoinType.LeftJoin)
                        f.LeftJoin<TempTable>(join.Table, join.Where);
                    else if (join.Type == JoinType.RightJoin)
                        f.RightJoin<TempTable>(join.Table, join.Where);
                    else
                        f.InnerJoin<TempTable>(join.Table, join.Where);
                }
            }

            QuerySection<TempTable> query = f.Select(creator.Fields).Where(creator.Where)
                    .OrderBy(creator.OrderBy);
            return query;
        }


        /// <summary>
        /// 创建一个参数方式的WhereClip
        /// </summary>
        /// <param name="field"></param>
        /// <param name="join"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static WhereClip CreateWhereClip(Field field, string join, object value)
        {
            if (value == null)
            {
                if (join == "=")
                    return new WhereClip(field.Name + " is null");
                else if (join == "<>")
                    return new WhereClip(field.Name + " is not null");
                else
                    throw new MySoftException("当值为null时只能应用于=与<>操作！");
            }

            string pName = DataUtils.MakeUniqueKey(30, "$p");
            SQLParameter p = new SQLParameter(pName);
            p.Value = value;

            string where = string.Format("{0} {1} {2}", field.Name, join, pName);

            return new WhereClip(where, p);
        }

        #endregion
    }
}