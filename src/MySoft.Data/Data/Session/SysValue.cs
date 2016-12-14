using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 系统值
    /// </summary>
    [Serializable]
    public class SysValue
    {
        private string sysvalue;
        public SysValue(string value)
        {
            this.sysvalue = value;
        }

        internal string Value
        {
            get { return sysvalue; }
        }
    }
}
