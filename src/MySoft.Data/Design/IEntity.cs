using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace MySoft.Data
{
    /// <summary>
    /// ʵ�����ӿ�
    /// </summary>
    public interface IEntityBase
    {
        /// <summary>
        /// ת������һ����
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        TEntity As<TEntity>();

        /// <summary>
        /// ����һ�����Ķ�����
        /// </summary>
        IRowReader ToRowReader();

        /// <summary>
        /// �����ֵ����
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> ToDictionary();

        /// <summary>
        /// ��¡һ������
        /// </summary>
        /// <returns></returns>
        EntityBase Clone();

        /// <summary>
        /// ��ȡ����״̬
        /// </summary>
        EntityState State { get; }

        /// <summary>
        /// ��ȡԭʼ����
        /// </summary>
        EntityBase Old { get; }
    }
}

namespace MySoft.Data.Design
{
    /// <summary>
    /// ʵ��ӿ�
    /// </summary>
    public interface IEntity : IEntityBase
    {
        #region ״̬����

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
