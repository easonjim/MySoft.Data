using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 字段及值
    /// </summary>
    internal class FieldValue
    {
        private Field field;
        public Field Field
        {
            get { return field; }
        }

        private object fvalue;
        public object Value
        {
            get { return fvalue; }
            internal set { fvalue = value; }
        }

        private bool isIdentity;
        public bool IsIdentity
        {
            get { return isIdentity; }
            set { isIdentity = value; }
        }

        private bool isPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set { isPrimaryKey = value; }
        }

        private bool isChanged;
        public bool IsChanged
        {
            get { return isChanged; }
            set { isChanged = value; }
        }

        /// <summary>
        /// 实例化FieldValue
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public FieldValue(Field field, object value)
        {
            this.field = field;
            this.fvalue = value;
        }
    }
}
