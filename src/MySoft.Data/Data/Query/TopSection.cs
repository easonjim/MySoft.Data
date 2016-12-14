using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySoft.Data
{
    /// <summary>
    /// Top对应的Query查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TopSection<T> : QuerySection<T>
        where T : Entity
    {
        private string topString;
        private int topSize;
        internal TopSection(string topString, FromSection<T> fromSection, DbProvider dbProvider, DbTrans dbTran, Field pagingField, int topSize)
            : base(fromSection, dbProvider, dbTran, pagingField)
        {
            this.topString = topString;
            this.topSize = topSize;
        }

        internal new string QueryString
        {
            get
            {
                return topString;
            }
        }

        #region 方法重载

        /// <summary>
        /// 返回结果列表
        /// </summary>
        /// <returns></returns>
        public override ArrayList<object> ToListResult()
        {
            return base.ToListResult(0, topSize);
        }

        /// <summary>
        /// 返回结果列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public override ArrayList<TResult> ToListResult<TResult>()
        {
            return base.ToListResult<TResult>(0, topSize);
        }

        /// <summary>
        /// 返回实体列表
        /// </summary>
        /// <returns></returns>
        public override SourceList<T> ToList()
        {
            return base.ToList(0, topSize);
        }

        /// <summary>
        /// 返回实体列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public override SourceList<TEntity> ToList<TEntity>()
        {
            return base.ToList<TEntity>(0, topSize);
        }

        /// <summary>
        /// 返回阅读器
        /// </summary>
        /// <returns></returns>
        public override SourceReader ToReader()
        {
            return base.ToReader(0, topSize);
        }

        /// <summary>
        /// 返回表数据
        /// </summary>
        /// <returns></returns>
        public override SourceTable ToTable()
        {
            return base.ToTable(0, topSize);
        }

        #endregion
    }
}
