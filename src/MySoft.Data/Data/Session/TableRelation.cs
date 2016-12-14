using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 关系表，可以用来存储关联信息
    /// </summary>
    public class TableRelation<T>
        where T : Entity
    {
        private FromSection<T> section;
        internal FromSection<T> Section
        {
            get { return section; }
        }

        internal TableRelation(Table table)
        {
            if (table == null)
                this.section = new FromSection<T>(null, null, Table.GetTable<T>());
            else
                this.section = new FromSection<T>(null, null, table);

            this.section.EntityList.Add(DataUtils.CreateInstance<T>());
        }

        #region 不带别名

        /// <summary>
        /// 左关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> LeftJoin<TJoin>(Table table, WhereClip onWhere)
            where TJoin : Entity
        {
            section.LeftJoin<TJoin>(table, onWhere);
            return this;
        }

        /// <summary>
        /// 右关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> RightJoin<TJoin>(Table table, WhereClip onWhere)
            where TJoin : Entity
        {
            section.RightJoin<TJoin>(table, onWhere);
            return this;
        }

        /// <summary>
        /// 内关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> InnerJoin<TJoin>(Table table, WhereClip onWhere)
            where TJoin : Entity
        {
            section.InnerJoin<TJoin>(table, onWhere);
            return this;
        }

        #endregion

        #region 不带别名

        /// <summary>
        /// 左关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> LeftJoin<TJoin>(WhereClip onWhere)
            where TJoin : Entity
        {
            section.LeftJoin<TJoin>(onWhere);
            return this;
        }

        /// <summary>
        /// 右关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> RightJoin<TJoin>(WhereClip onWhere)
            where TJoin : Entity
        {
            section.RightJoin<TJoin>(onWhere);
            return this;
        }

        /// <summary>
        /// 内关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> InnerJoin<TJoin>(WhereClip onWhere)
            where TJoin : Entity
        {
            section.InnerJoin<TJoin>(onWhere);
            return this;
        }

        #endregion

        #region 带别名

        /// <summary>
        /// 左关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> LeftJoin<TJoin>(string aliasName, WhereClip onWhere)
            where TJoin : Entity
        {
            section.LeftJoin<TJoin>(aliasName, onWhere);
            return this;
        }

        /// <summary>
        /// 右关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> RightJoin<TJoin>(string aliasName, WhereClip onWhere)
            where TJoin : Entity
        {
            section.RightJoin<TJoin>(aliasName, onWhere);
            return this;
        }

        /// <summary>
        /// 内关联
        /// </summary>
        /// <param name="table"></param>
        /// <param name="onWhere"></param>
        /// <returns></returns>
        public TableRelation<T> InnerJoin<TJoin>(string aliasName, WhereClip onWhere)
            where TJoin : Entity
        {
            section.InnerJoin<TJoin>(aliasName, onWhere);
            return this;
        }

        #endregion

        #region 创建子查询

        /// <summary>
        /// 生成一个子查询
        /// </summary>
        /// <returns></returns>
        public TableRelation<T> SubQuery()
        {
            section.SetQuerySection(section.SubQuery());
            return this;
        }

        /// <summary>
        /// 生成一个子查询
        /// </summary>
        /// <returns></returns>
        public TableRelation<T> SubQuery(string aliasName)
        {
            section.SetQuerySection(section.SubQuery(aliasName));
            return this;
        }

        /// <summary>
        /// 生成一个子查询
        /// </summary>
        /// <returns></returns>
        public TableRelation<TSub> SubQuery<TSub>()
            where TSub : Entity
        {
            TableRelation<TSub> tr = new TableRelation<TSub>(null);
            tr.section.SetQuerySection(section.SubQuery<TSub>());
            return tr;
        }

        /// <summary>
        /// 生成一个带别名的子查询
        /// </summary>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public TableRelation<TSub> SubQuery<TSub>(string aliasName)
            where TSub : Entity
        {
            TableRelation<TSub> tr = new TableRelation<TSub>(null);
            tr.section.SetQuerySection(section.SubQuery<TSub>(aliasName));
            return tr;
        }

        #endregion

        /// <summary>
        /// 进行条件操作
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public TableRelation<T> Where(WhereClip where)
        {
            section.Where(where);
            return this;
        }

        /// <summary>
        /// 进行排序操作
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public TableRelation<T> OrderBy(OrderByClip orderBy)
        {
            section.OrderBy(orderBy);
            return this;
        }

        /// <summary>
        /// 进行分组操作
        /// </summary>
        /// <param name="groupBy"></param>
        /// <returns></returns>
        public TableRelation<T> GroupBy(GroupByClip groupBy)
        {
            section.GroupBy(groupBy);
            return this;
        }

        /// <summary>
        /// 进行查询操作
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public TableRelation<T> Select(params Field[] fields)
        {
            section.Select(fields);
            return this;
        }
    }
}
