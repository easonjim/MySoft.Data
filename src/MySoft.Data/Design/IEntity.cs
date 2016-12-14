using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace MySoft.Data
{
    /// <summary>
    /// 实体基类接口
    /// </summary>
    public interface IEntityBase
    {
        /// <summary>
        /// 转换成另一对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        TEntity As<TEntity>();

        /// <summary>
        /// 返回一个行阅读对象
        /// </summary>
        IRowReader ToRowReader();

        /// <summary>
        /// 返回字典对象
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> ToDictionary();

        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <returns></returns>
        EntityBase Clone();

        /// <summary>
        /// 获取对象状态
        /// </summary>
        EntityState State { get; }

        /// <summary>
        /// 获取原始对象
        /// </summary>
        EntityBase Old { get; }
    }
}

namespace MySoft.Data.Design
{
    /// <summary>
    /// 实体接口
    /// </summary>
    public interface IEntity : IEntityBase
    {
        #region 状态操作

        void Attach();
        void Attach(params Field[] removeFields);

        void Detach();
        void Detach(params Field[] removeFields);

        void AttachAll();
        void AttachAll(params Field[] removeFields);

        void DetachAll();
        void DetachAll(params Field[] removeFields);

        void AttachSet(params Field[] setFields);
        void DetachSet(params Field[] setFields);

        #endregion
    }
}
