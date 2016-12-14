using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;

namespace MySoft.Data
{
    /// <summary>
    /// 对任意类型添加文字描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumDescriptionAttribute : Attribute
    {
        private string _desc;

        public EnumDescriptionAttribute(string desc)
        {
            _desc = desc;
        }

        public string Description
        {
            get
            {
                return _desc;
            }
        }

        /// <summary>
        /// 获取枚举类型的描述
        /// </summary>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static string GetDescription(object enumObj)
        {
            Type enumType = enumObj.GetType();
            if (!enumType.IsEnum)
            {
                throw new Exception("参数不是枚举类型！");
            }
            FieldInfo fieldInfo = enumType.GetField(enumObj.ToString());
            object[] attribArray = fieldInfo.GetCustomAttributes(false);
            if (attribArray.Length == 0) return null;
            EnumDescriptionAttribute attrib = attribArray[0] as EnumDescriptionAttribute;
            return attrib.Description;
        }

        /// <summary>
        /// 获取枚举类型的所有描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string[] GetDescriptions(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("参数不是枚举类型！");
            }
            FieldInfo[] fieldInfos = enumType.GetFields();
            List<string> deslist = new List<string>();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray.Length == 0) continue;
                EnumDescriptionAttribute attrib = attribArray[0] as EnumDescriptionAttribute;
                deslist.Add(attrib.Description);
            }
            return deslist.ToArray();
        }
    }
}