using System;
using System.Collections.Generic;
using System.Text;
using MySoft.Data.Design;
using System.Data;

namespace MySoft.Data
{
    /// <summary>
    /// Entity状态
    /// </summary>
    [Serializable]
    public enum EntityState
    {
        /// <summary>
        /// 插入状态
        /// </summary>
        Insert,
        /// <summary>
        /// 修改状态
        /// </summary>
        Update
    }

    /// <summary>
    /// Entity基类
    /// </summary>
    [Serializable]
    public abstract class EntityBase : IEntityBase
    {
        protected List<Field> updatelist = new List<Field>();
        protected List<Field> removeinsertlist = new List<Field>();
        protected EntityBase originalObject;
        protected bool isUpdate = false;
        protected bool isFromDB = false;

        /// <summary>
        /// 转换成另一对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity As<TEntity>()
        {
            lock (this)
            {
                if (this is TEntity)
                    return DataUtils.ConvertType<IEntityBase, TEntity>(this);

                return default(TEntity);
            }
        }


        /// <summary>
        /// 返回一个行阅读对象
        /// </summary>
        IRowReader IEntityBase.ToRowReader()
        {
            lock (this)
            {
                try
                {
                    SourceList<EntityBase> list = new SourceList<EntityBase>();
                    list.Add(this);

                    DataTable dt = list.GetDataTable(this.GetType());
                    ISourceTable table = new SourceTable(dt);
                    return table[0];
                }
                catch (Exception ex)
                {
                    throw new MySoftException("数据转换失败！", ex);
                }
            }
        }

        /// <summary>
        /// 返回字典对象
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> IEntityBase.ToDictionary()
        {
            try
            {
                IDictionary<string, object> dict = new Dictionary<string, object>();
                foreach (Field f in GetFields())
                {
                    object value = DataUtils.GetPropertyValue(this, f.PropertyName);
                    dict[f.OriginalName] = value;
                }
                return dict;
            }
            catch (Exception ex)
            {
                throw new MySoftException("数据转换失败！", ex);
            }
        }

        /// <summary>
        /// 获取原始对象
        /// </summary>
        EntityBase IEntityBase.Old
        {
            get
            {
                return originalObject;
            }
        }

        /// <summary>
        /// 获取对象状态
        /// </summary>
        EntityState IEntityBase.State
        {
            get
            {
                return isUpdate ? EntityState.Update : EntityState.Insert;
            }
        }

        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <returns></returns>
        EntityBase IEntityBase.Clone()
        {
            lock (this)
            {
                return DataUtils.CloneEntity(this);
            }
        }

        #region 字段信息

        /// <summary>
        /// 返回标识列的名称（如Oracle中Sequence.nextval）
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSequence()
        {
            return null;
        }

        /// <summary>
        /// 获取标识列
        /// </summary>
        /// <returns></returns>
        protected virtual Field GetIdentityField()
        {
            return null;
        }

        /// <summary>
        /// 获取主键列表
        /// </summary>
        /// <returns></returns>
        protected virtual Field[] GetPrimaryKeyFields()
        {
            return new Field[] { };
        }

        /// <summary>
        /// 获取字段列表
        /// </summary>
        /// <returns></returns>
        internal protected abstract Field[] GetFields();

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <returns></returns>
        protected abstract object[] GetValues();

        #endregion

        /// <summary>
        /// 获取只读属性
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool GetReadOnly()
        {
            return false;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        protected internal virtual Table GetTable()
        {
            return new Table("TempTable");
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="reader"></param>
        protected abstract void SetValues(IRowReader reader);

        /// <summary>
        /// 用于设置额外的值
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void SetPropertyValues(IRowReader reader)
        { }

        #region 内部方法

        internal bool IsUpdate
        {
            get
            {
                lock (this)
                {
                    return isUpdate;
                }
            }
        }

        /// <summary>
        /// 获取系列的名称
        /// </summary>
        internal string SequenceName
        {
            get
            {
                lock (this)
                {
                    return this.GetSequence();
                }
            }
        }

        /// <summary>
        /// 获取排序或分页字段
        /// </summary>
        /// <returns></returns>
        internal Field PagingField
        {
            get
            {
                lock (this)
                {
                    Field pagingField = this.GetIdentityField();

                    if ((IField)pagingField == null)
                    {
                        Field[] fields = this.GetPrimaryKeyFields();
                        if (fields.Length > 0) pagingField = fields[0];
                    }

                    return pagingField;
                }
            }
        }

        /// <summary>
        /// 获取标识列
        /// </summary>
        internal Field IdentityField
        {
            get
            {
                lock (this)
                {
                    return this.GetIdentityField();
                }
            }
        }

        /// <summary>
        /// 设置所有的值
        /// </summary>
        /// <param name="reader"></param>
        internal void SetAllValues(IRowReader reader)
        {
            lock (this)
            {
                //设置内部的值
                SetValues(reader);

                //设置外部的值
                SetPropertyValues(reader);

                //设置来自数据库变量为true
                isFromDB = true;
            }
        }

        /// <summary>
        /// 获取字段及值
        /// </summary>
        /// <returns></returns>
        internal List<FieldValue> GetFieldValues()
        {
            lock (this)
            {
                List<FieldValue> fvlist = new List<FieldValue>();

                Field identityField = this.GetIdentityField();
                List<Field> pkFields = new List<Field>(this.GetPrimaryKeyFields());

                Field[] fields = this.GetFields();
                object[] values = this.GetValues();

                if (fields.Length != values.Length)
                {
                    throw new MySoftException("字段与值无法对应！"); ;
                }

                int index = 0;
                foreach (Field field in fields)
                {
                    FieldValue fv = new FieldValue(field, values[index]);

                    //判断是否为标识列
                    if ((IField)identityField != null)
                        if (identityField.Name == field.Name) fv.IsIdentity = true;

                    //判断是否为主键
                    if (pkFields.Contains(field)) fv.IsPrimaryKey = true;

                    if (isUpdate)
                    {
                        //如果是更新，则将更新的字段改变状态为true
                        if (updatelist.Contains(field)) fv.IsChanged = true;
                    }
                    else
                    {
                        //如果是插入，则将移除插入的字段改变状态为true
                        if (removeinsertlist.Contains(field)) fv.IsChanged = true;
                    }

                    fvlist.Add(fv);
                    index++;
                }

                return fvlist;
            }
        }

        #endregion
    }
}
