using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data.Design
{
    /// <summary>
    /// 定义自增长列
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class SequenceAttribute : Attribute
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        public SequenceAttribute(string name)
        {
            this.name = name;
        }
    }
}
