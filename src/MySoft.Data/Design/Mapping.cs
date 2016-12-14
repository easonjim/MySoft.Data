using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data.Design
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public sealed class MappingAttribute : Attribute
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        public MappingAttribute(string name)
        {
            this.name = name;
        }
    }
}
