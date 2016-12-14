using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data.Design
{
    /// <summary>
    /// 
    /// </summary>
    public enum ReadOnlyType
    {
        /// <summary>
        /// 只读实体，只用于输出
        /// </summary>
        Entity,
        /// <summary>
        /// 数据库视图
        /// </summary>
        View
    }

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class ReadOnlyAttribute : Attribute
    {
        private ReadOnlyType type;
        public ReadOnlyType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public ReadOnlyAttribute()
        {
            this.type = ReadOnlyType.View;
        }
    }
}
