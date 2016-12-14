using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// TableJoin
    /// </summary>
    [Serializable]
    internal class TableJoin
    {
        public Table Table { get; set; }
        public JoinType Type { get; set; }
        public WhereClip Where { get; set; }
    }

    /// <summary>
    /// 查询创建器
    /// </summary>
    [Serializable]
    public class QueryCreator : IQueryCreator
    {
        /// <summary>
        /// 创建一个新的查询器（条件为全部，排序为默认)
        /// </summary>
        public static QueryCreator NewCreator()
        {
            return new QueryCreator();
        }

        private Table table;
        private IDictionary<string, TableJoin> joinTables;
        private IList<WhereClip> whereList;
        private List<OrderByClip> orderList;
        private List<Field> fieldList;

        /// <summary>
        /// 实例化QueryCreater
        /// </summary>
        protected QueryCreator()
        {
            this.whereList = new List<WhereClip>();
            this.orderList = new List<OrderByClip>();
            this.fieldList = new List<Field>();
            this.joinTables = new Dictionary<string, TableJoin>();
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
        /// 返回条件
        /// </summary>
        internal WhereClip Where
        {
            get
            {
                WhereClip newWhere = WhereClip.All;
                foreach (WhereClip where in whereList)
                {
                    newWhere &= where;
                }
                return newWhere;
            }
        }

        /// <summary>
        /// 返回排序
        /// </summary>
        internal OrderByClip OrderBy
        {
            get
            {
                OrderByClip newOrder = OrderByClip.Default;
                foreach (OrderByClip order in orderList)
                {
                    newOrder &= order;
                }
                return newOrder;
            }
        }

        /// <summary>
        /// 返回字段列表
        /// </summary>
        internal Field[] Fields
        {
            get
            {
                if (fieldList.Count == 0)
                {
                    return new Field[] { Field.All.At(table) };
                }
                return fieldList.ToArray();
            }
        }

        /// <summary>
        /// 是否关联查询
        /// </summary>
        internal bool IsRelation
        {
            get
            {
                return joinTables.Count > 0;
            }
        }

        /// <summary>
        /// 获取关联信息
        /// </summary>
        internal IDictionary<string, TableJoin> Relations
        {
            get
            {
                return joinTables;
            }
        }

        #endregion

        #region 设置表信息

        /// <summary>
        /// 设置表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public QueryCreator From<T>()
            where T : Entity
        {
            this.table = Table.GetTable<T>();
            return this;
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        public QueryCreator From(string tableName)
        {
            this.table = new Table(tableName);
            return this;
        }

        /// <summary>
        /// 设置表信息
        /// </summary>
        /// <param name="table"></param>
        public QueryCreator From(Table table)
        {
            this.table = table;
            return this;
        }

        #region 表关联处理

        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public QueryCreator Join(string tableName, string where, params SQLParameter[] parameters)
        {
            return Join(JoinType.LeftJoin, tableName, where, parameters);
        }

        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        public QueryCreator Join(Table table, WhereClip where)
        {
            return Join(JoinType.LeftJoin, table, where);
        }

        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public QueryCreator Join<T>(string where, params SQLParameter[] parameters)
            where T : Entity
        {
            return Join<T>(JoinType.LeftJoin, where, parameters);
        }

        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        public QueryCreator Join<T>(WhereClip where)
              where T : Entity
        {
            return Join<T>(JoinType.LeftJoin, where);
        }


        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public QueryCreator Join(JoinType joinType, string tableName, string where, params SQLParameter[] parameters)
        {
            Table t = new Table(tableName);
            if (!this.joinTables.ContainsKey(t.OriginalName))
            {
                TableJoin join = new TableJoin()
                {
                    Table = t,
                    Type = joinType,
                    Where = new WhereClip(where, parameters)
                };
                this.joinTables.Add(t.OriginalName, join);
            }
            return this;
        }

        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        public QueryCreator Join(JoinType joinType, Table table, WhereClip where)
        {
            if (!this.joinTables.ContainsKey(table.OriginalName))
            {
                TableJoin join = new TableJoin()
                {
                    Table = table,
                    Type = JoinType.LeftJoin,
                    Where = where
                };
                this.joinTables.Add(table.OriginalName, join);
            }
            return this;
        }

        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public QueryCreator Join<T>(JoinType joinType, string where, params SQLParameter[] parameters)
            where T : Entity
        {
            return Join(joinType, Table.GetTable<T>().OriginalName, where, parameters);
        }

        /// <summary>
        /// 关联表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        public QueryCreator Join<T>(JoinType joinType, WhereClip where)
              where T : Entity
        {
            return Join(joinType, Table.GetTable<T>(), where);
        }

        #endregion

        #endregion

        #region 增加一个条件

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="where"></param>
        public QueryCreator AddWhere(WhereClip where)
        {
            if (DataUtils.IsNullOrEmpty(where)) return this;

            //不存在条件，则加入
            whereList.Add(where);

            return this;
        }

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public QueryCreator AddWhere(string where, params SQLParameter[] parameters)
        {
            if (string.IsNullOrEmpty(where)) return this;

            return AddWhere(new WhereClip(where, parameters));
        }

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public QueryCreator AddWhere(Field field, object value)
        {
            if (value == null)
                return AddWhere(field.IsNull());
            else
                return AddWhere(field == value);
        }

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public QueryCreator AddWhere(string fieldName, object value)
        {
            return AddWhere(new Field(fieldName), value);
        }

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public QueryCreator AddWhere(string tableName, string fieldName, object value)
        {
            return AddWhere(new Field(fieldName).At(new Table(tableName)), value);
        }

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public QueryCreator AddWhere<T>(string fieldName, object value)
            where T : MySoft.Data.Entity
        {
            string tableName = Table.GetTable<T>().OriginalName;
            return AddWhere(tableName, fieldName, value);
        }

        #endregion

        #region 增加一个排序

        /// <summary>
        /// 添加一个排序
        /// </summary>
        /// <param name="order"></param>
        public QueryCreator AddOrder(OrderByClip order)
        {
            if (DataUtils.IsNullOrEmpty(order)) return this;

            if (orderList.Exists(o =>
            {
                return string.Compare(order.ToString(), o.ToString()) == 0;
            }))
            {
                return this;
            }

            //不存在条件，则加入
            orderList.Add(order);

            return this;
        }

        /// <summary>
        /// 添加一个排序
        /// </summary>
        /// <param name="orderby"></param>
        public QueryCreator AddOrder(string orderby)
        {
            if (string.IsNullOrEmpty(orderby)) return this;

            return AddOrder(new OrderByClip(orderby));
        }

        /// <summary>
        /// 增加排序规则
        /// </summary>
        /// <param name="field"></param>
        /// <param name="desc"></param>
        public QueryCreator AddOrder(Field field, bool desc)
        {
            if (desc)
                return AddOrder(field.Desc);
            else
                return AddOrder(field.Asc);
        }

        /// <summary>
        /// 增加排序规则
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="desc"></param>
        public QueryCreator AddOrder(string fieldName, bool desc)
        {
            return AddOrder(new Field(fieldName), desc);
        }

        /// <summary>
        /// 增加排序规则
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="desc"></param>
        public QueryCreator AddOrder(string tableName, string fieldName, bool desc)
        {
            return AddOrder(new Field(fieldName).At(new Table(tableName)), desc);
        }

        /// <summary>
        /// 增加排序规则
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="desc"></param>
        public QueryCreator AddOrder<T>(string fieldName, bool desc)
            where T : MySoft.Data.Entity
        {
            string tableName = Table.GetTable<T>().OriginalName;
            return AddOrder(tableName, fieldName, desc);
        }

        #endregion

        #region 增加字段

        /// <summary>
        /// 添加一个字段
        /// </summary>
        /// <param name="field"></param>
        public QueryCreator AddField(Field field)
        {
            if (fieldList.Exists(f =>
            {
                return string.Compare(field.Name, f.Name) == 0;
            }))
            {
                return this;
            }

            fieldList.Add(field);

            return this;
        }

        /// <summary>
        /// 添加一个字段
        /// </summary>
        /// <param name="fieldName"></param>
        public QueryCreator AddField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return this;

            return AddField(new Field(fieldName));
        }

        /// <summary>
        /// 添加一个字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        public QueryCreator AddField(string tableName, string fieldName)
        {
            return AddField(new Field(fieldName).At(new Table(tableName)));
        }

        /// <summary>
        /// 添加一个字段
        /// </summary>
        /// <param name="fieldName"></param>
        public QueryCreator AddField<T>(string fieldName)
            where T : MySoft.Data.Entity
        {
            string tableName = Table.GetTable<T>().OriginalName;
            return AddField(tableName, fieldName);
        }

        /// <summary>
        /// 移除指定的列
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public QueryCreator RemoveField(params Field[] fields)
        {
            if (fields == null) return this;

            foreach (Field field in fields)
            {
                this.fieldList.RemoveAll(f =>
                {
                    return string.Compare(f.OriginalName, field.OriginalName, true) == 0;
                });
            }

            return this;
        }

        /// <summary>
        /// 移除指定的列
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        public QueryCreator RemoveField(params string[] fieldNames)
        {
            if (fieldNames == null) return this;

            foreach (string fieldName in fieldNames)
            {
                this.fieldList.RemoveAll(f =>
                {
                    return string.Compare(f.OriginalName, fieldName, true) == 0;
                });
            }

            return this;
        }

        #endregion
    }
}
