using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 删除创建器
    /// </summary>
    [Serializable]
    public class DeleteCreator : IDeleteCreator
    {
        /// <summary>
        /// 创建一个新的删除器
        /// </summary>
        public static DeleteCreator NewCreator()
        {
            return new DeleteCreator();
        }

        private Table table;
        private IList<WhereClip> whereList;

        /// <summary>
        /// 实例化DeleteCreator
        /// </summary>
        protected DeleteCreator()
        {
            this.whereList = new List<WhereClip>();
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

        #endregion

        #region 设置表信息

        /// <summary>
        /// 设置表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DeleteCreator From<T>()
            where T : Entity
        {
            this.table = Table.GetTable<T>();
            return this;
        }

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        public DeleteCreator From(string tableName)
        {
            this.table = new Table(tableName);
            return this;
        }

        /// <summary>
        /// 设置表信息
        /// </summary>
        /// <param name="table"></param>
        public DeleteCreator From(Table table)
        {
            this.table = table;
            return this;
        }

        #endregion

        #region 增加一个条件

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="where"></param>
        public DeleteCreator AddWhere(WhereClip where)
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
        public DeleteCreator AddWhere(string where, params SQLParameter[] parameters)
        {
            if (string.IsNullOrEmpty(where)) return this;

            return AddWhere(new WhereClip(where, parameters));
        }

        /// <summary>
        /// 添加一个条件
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public DeleteCreator AddWhere(Field field, object value)
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
        public DeleteCreator AddWhere(string fieldName, object value)
        {
            return AddWhere(new Field(fieldName), value);
        }

        #endregion
    }
}
