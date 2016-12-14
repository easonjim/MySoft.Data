using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 临时的TResult类
    /// </summary>
    [Serializable]
    internal class TempTable : Entity
    {
        internal protected override Field[] GetFields()
        {
            return new Field[] { };
        }

        protected override object[] GetValues()
        {
            return new object[] { };
        }

        protected override void SetValues(IRowReader reader)
        { }
    }
}
