using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 排序属性
    /// </summary>
    public class SortProperty
    {
        private string propertyName;
        internal string PropertyName
        {
            get { return propertyName; }
        }

        private bool desc;
        internal bool IsDesc
        {
            get { return desc; }
        }

        public SortProperty(string propertyName)
        {
            this.propertyName = propertyName;
            this.desc = false;
        }

        private SortProperty(string propertyName, bool desc)
            : this(propertyName)
        {
            this.desc = desc;
        }

        /// <summary>
        /// 从小到大
        /// </summary>
        public SortProperty Asc
        {
            get
            {
                return new SortProperty(this.propertyName, false);
            }
        }

        /// <summary>
        /// 从大到小
        /// </summary>
        public SortProperty Desc
        {
            get
            {
                return new SortProperty(this.propertyName, true);
            }
        }
    }

    /// <summary>
    /// 自定义排序算法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortComparer<T> : IComparer<T>
    {
        private List<SortProperty> sorts;
        /// <summary>
        /// 初始化自定义比较类
        /// </summary>
        /// <param name="sorts"></param>
        public SortComparer(params SortProperty[] sorts)
        {
            this.sorts = new List<SortProperty>(sorts);
        }

        /// <summary>
        /// 添加排序属性
        /// </summary>
        /// <param name="sorts"></param>
        public void AddProperty(params SortProperty[] sorts)
        {
            if (sorts != null && sorts.Length > 0)
            {
                this.sorts.AddRange(sorts);
            }
        }

        /// <summary>
        /// 实现Compare比较两个值的大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            return CompareValue(x, y, 0);
        }

        #region 值比较

        /// <summary>
        /// 进行深层排序
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private int CompareValue(T x, T y, int index)
        {
            int ret = 0;
            if (sorts.Count - 1 >= index)
            {
                ret = CompareProperty(x, y, sorts[index]);
                if (ret == 0)
                {
                    ret = CompareValue(x, y, ++index);
                }
            }

            return ret;
        }

        /// <summary>
        /// 比较两个值的大小(从小到大)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private int CompareProperty(T x, T y, SortProperty property)
        {
            object value1 = DataUtils.GetPropertyValue(x, property.PropertyName);
            object value2 = DataUtils.GetPropertyValue(y, property.PropertyName);

            int ret = 0;
            if (value1 == null && value2 == null) ret = 0;
            else if (value1 == null) ret = -1;
            else if (value2 == null) ret = 1;
            else if (value1.GetType().IsGenericType && value1.GetType().GetGenericTypeDefinition() == typeof(Nullable<>)
                && value2.GetType().IsGenericType && value2.GetType().GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                //如果是Nullable<>类型，需要特殊处理
                Type type1 = Nullable.GetUnderlyingType(value1.GetType());
                Type type2 = Nullable.GetUnderlyingType(value2.GetType());
                value1 = Convert.ChangeType(value1, type1);
                value2 = Convert.ChangeType(value2, type2);
                ret = ((IComparable)value1).CompareTo((IComparable)value2);
            }
            else ret = ((IComparable)value1).CompareTo((IComparable)value2);

            if (property.IsDesc) return -ret;
            return ret;
        }

        #endregion
    }
}
