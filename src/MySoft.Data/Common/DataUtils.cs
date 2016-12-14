using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;

namespace MySoft.Data
{
    /// <summary>
    /// ���ݷ�����
    /// </summary>
    public static class DataUtils
    {
        #region �ⲿ����

        /// <summary>
        /// ��¡һ������
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object CloneObject(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// ��¡һ��Entity����
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T CloneEntity<T>(T entity)
            where T : EntityBase
        {
            if (entity == null) return default(T);
            Type type = entity.GetType();
            object t = GetFastInstanceCreator(type)();
            var fiels = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            var list = new List<FieldInfo>(fiels);
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (!CanUseType(p.PropertyType)) continue; //shallow only
                object value = GetPropertyValue(entity, p.Name);
                if (value == null) continue;

                //�����Ը�ֵ
                FieldInfo field = list.Find(f => f.Name == "_" + p.Name);
                if (field != null) field.SetValue(t, value);
            }

            return (T)t;
        }

        private static bool CanUseType(Type propertyType)
        {
            //only strings and value types
            if (propertyType.IsArray) return false;
            if (!propertyType.IsValueType && propertyType != typeof(string)) return false;
            return true;
        }

        /// <summary>
        /// �Ӷ���obj�л�ȡֵ������ǰʵ��,TOutput����Ϊclass��ӿ�
        /// TInput����Ϊclass��IRowReader��NameValueCollection
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TOutput ConvertType<TInput, TOutput>(TInput obj)
        {
            if (obj == null) return default(TOutput);

            if (obj is TOutput)
            {
                return (TOutput)(obj as object);
            }
            else
            {
                try
                {
                    TOutput t = CreateInstance<TOutput>();
                    foreach (PropertyInfo p in typeof(TOutput).GetProperties())
                    {
                        object value = null;
                        if (obj is IRowReader)
                        {
                            IRowReader reader = obj as IRowReader;
                            if (reader.IsDBNull(p.Name)) continue;
                            value = reader[p.Name];
                        }
                        else if (obj is NameValueCollection)
                        {
                            NameValueCollection reader = obj as NameValueCollection;
                            if (reader[p.Name] == null) continue;
                            value = reader[p.Name];
                        }
                        else
                        {
                            value = GetPropertyValue(obj, p.Name);
                        }
                        if (value == null) continue;
                        SetPropertyValue(t, p, value);
                    }

                    return t;
                }
                catch (MySoftException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw new MySoftException(ex.Message);
                }
            }
        }

        /// <summary>
        /// ת����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertValue<T>(object value)
        {
            if (value == DBNull.Value || value == null)
                return default(T);

            if (value is T)
            {
                return (T)value;
            }
            else
            {
                object obj = ConvertValue(typeof(T), value);
                if (obj == null)
                {
                    return default(T);
                }
                return (T)obj;
            }
        }

        /// <summary>
        /// ת����������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ConvertValue(Type type, object value)
        {
            if (value == DBNull.Value || value == null)
                return null;

            if (CheckStruct(type))
            {
                //����ֶ�Ϊ�ṹ�������ϵ�л�����
                return SerializationManager.Deserialize(type, value.ToString());
            }
            else
            {
                Type valueType = value.GetType();

                //�����ǰֵ�Ǵ�����Type����
                if (type.IsAssignableFrom(valueType))
                {
                    return value;
                }
                else
                {
                    if (type.IsEnum)
                    {
                        try
                        {
                            return Enum.Parse(type, value.ToString(), true);
                        }
                        catch
                        {
                            return Enum.ToObject(type, value);
                        }
                    }
                    else
                    {
                        return ChangeType(value, type);
                    }
                }
            }
        }

        #region ���Բ���

        /// <summary>
        /// ��ȡ�Զ�������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static T GetPropertyAttribute<T>(PropertyInfo pi)
        {
            object[] attrs = pi.GetCustomAttributes(typeof(T), false);
            if (attrs != null && attrs.Length > 0)
            {
                return (T)attrs[0];
            }
            return default(T);
        }

        /// <summary>
        /// ��ȡ�Զ�������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetTypeAttribute<T>(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(T), false);
            if (attrs != null && attrs.Length > 0)
            {
                return (T)attrs[0];
            }
            return default(T);
        }

        #endregion

        /// <summary>
        /// �ж�WhereClip�Ƿ�Ϊnull���
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(WhereClip where)
        {
            if ((object)where == null || string.IsNullOrEmpty(where.ToString()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �ж�OrderByClip�Ƿ�Ϊnull���
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(OrderByClip order)
        {
            if ((object)order == null || string.IsNullOrEmpty(order.ToString()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �ж�GroupByClip�Ƿ�Ϊnull���
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(GroupByClip group)
        {
            if ((object)group == null || string.IsNullOrEmpty(group.ToString()))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region DynamicCalls

        /// <summary>
        /// ���ٴ���һ��T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            return (T)GetFastInstanceCreator(typeof(T))();
        }

        /// <summary>
        /// ����һ��ί��
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FastCreateInstanceHandler GetFastInstanceCreator(Type type)
        {
            if (type.IsInterface)
            {
                throw new MySoftException("��ʵ�����Ķ������Ͳ����ǽӿڣ�");
            }
            FastCreateInstanceHandler creator = DynamicCalls.GetInstanceCreator(type);
            return creator;
        }

        /// <summary>
        /// ���ٵ��÷���
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static FastInvokeHandler GetFastMethodInvoke(MethodInfo method)
        {
            FastInvokeHandler invoke = DynamicCalls.GetMethodInvoker(method);
            return invoke;
        }

        #endregion

        #region ���Ը�ֵ��ȡֵ

        /// <summary>
        /// ������������ֵ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(object obj, PropertyInfo property, object value)
        {
            if (!property.CanWrite) return;
            try
            {
                FastPropertySetHandler setter = DynamicCalls.GetPropertySetter(property);
                value = ConvertValue(property.PropertyType, value);
                setter(obj, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ������������ֵ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                SetPropertyValue(obj, property, value);
            }
        }

        /// <summary>
        /// ���ٻ�ȡ����ֵ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, PropertyInfo property)
        {
            if (!property.CanRead) return null;
            FastPropertyGetHandler getter = DynamicCalls.GetPropertyGetter(property);
            return getter(obj);
        }

        /// <summary>
        /// ���ٻ�ȡ����ֵ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return GetPropertyValue(obj, property);
            }
            return null;
        }

        #endregion

        #region �ڲ�����

        /// <summary>
        /// ��ʽ������Ϊ���ݿ�ͨ�ø�ʽ
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        internal static string FormatValue(object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return "null";
            }

            Type type = val.GetType();

            if (type == typeof(Guid))
            {
                return string.Format("'{0}'", val);
            }
            else if (type == typeof(DateTime))
            {
                return string.Format("'{0}'", ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (type == typeof(bool))
            {
                return ((bool)val) ? "1" : "0";
            }
            else if (val is Field)
            {
                return ((Field)val).Name;
            }
            else if (val is SysValue)
            {
                return ((SysValue)val).Value;
            }
            else if (type.IsEnum)
            {
                return Convert.ToInt32(val).ToString();
            }
            else if (type.IsValueType)
            {
                if (CheckStruct(type))
                {
                    //���������ֵ���ͣ������ϵ�л��洢
                    return SerializationManager.Serialize(val);
                }

                return val.ToString();
            }
            else
            {
                return string.Format("N'{0}'", val.ToString());
            }
        }

        internal static string FormatSQL(string sql, char leftToken, char rightToken, bool isAccess)
        {
            if (sql == null) return string.Empty;

            try
            {
                if (isAccess)
                    sql = string.Format(sql, leftToken, rightToken, '(', ')');
                else
                    sql = string.Format(sql, leftToken, rightToken, ' ', ' ');
            }
            catch
            {
                if (isAccess)
                    sql = sql.Replace("{0}", leftToken.ToString()).Replace("{1}", rightToken.ToString()).Replace("{2}", '('.ToString()).Replace("{3}", ')'.ToString());
                else
                    sql = sql.Replace("{0}", leftToken.ToString()).Replace("{1}", rightToken.ToString()).Replace("{2}", ' '.ToString()).Replace("{3}", ' '.ToString());
            }

            return sql.Trim().Replace(" . ", ".")
                            .Replace(" , ", ",")
                            .Replace(" ( ", " (")
                            .Replace(" ) ", ") ")
                            .Replace("   ", " ")
                            .Replace("  ", " ");
        }

        internal static object[] CheckAndReturnValues(object[] values)
        {
            //���ֵΪnull���򷵻ز�������
            if (values == null)
            {
                throw new MySoftException("��������ݲ���Ϊnull��");
            }

            //�������Ϊ0���򷵻ز�������
            if (values.Length == 0)
            {
                throw new MySoftException("��������ݸ�������Ϊ0��");
            }

            //����������Ͳ���object,��ǿ��ת��
            if (values.Length == 1 && values[0].GetType().IsArray)
            {
                try
                {
                    values = ArrayList.Adapter((Array)values[0]).ToArray();
                }
                catch
                {
                    throw new MySoftException("��������ݲ�����ȷ��������");
                }
            }

            return values;
        }

        internal static WhereClip GetPkWhere<T>(Table table, object[] pkValues)
            where T : Entity
        {
            WhereClip where = null;
            List<FieldValue> list = CreateInstance<T>().GetFieldValues();
            pkValues = CheckAndReturnValues(pkValues);


            for (int i = 0; i < pkValues.Length; i++)
            {
                list.ForEach(fv =>
                {

                    if (fv.IsPrimaryKey)
                    {
                        if (pkValues.Length == 1)
                            where &= fv.Field.At(table) == pkValues[i];
                        else
                            where |= fv.Field.At(table) == pkValues[i];

                    }
                });
            }


            return where;
        }

        internal static WhereClip GetPkWhere<T>(Table table, T entity)
            where T : Entity
        {
            WhereClip where = null;
            List<FieldValue> list = entity.GetFieldValues();

            list.ForEach(fv =>
            {
                if (fv.IsPrimaryKey)
                {
                    where &= fv.Field.At(table) == fv.Value;
                }
            });

            return where;
        }

        /// <summary>
        /// ����һ��FieldValue�б�
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static List<FieldValue> CreateFieldValue(Field[] fields, object[] values, bool isInsert)
        {
            if (fields == null || values == null)
            {
                throw new MySoftException("�ֶμ�ֵ����Ϊnull��");
            }

            if (fields.Length != values.Length)
            {
                throw new MySoftException("�ֶμ�ֵ���Ȳ�һ�£�");
            }

            int index = 0;
            List<FieldValue> fvlist = new List<FieldValue>();
            foreach (Field field in fields)
            {
                FieldValue fv = new FieldValue(field, values[index]);

                if (isInsert && values[index] is Field)
                {
                    fv.IsIdentity = true;
                }
                else if (!isInsert)
                {
                    fv.IsChanged = true;
                }

                fvlist.Add(fv);

                index++;
            }

            return fvlist;
        }

        /// <summary>
        /// Makes a unique key.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        internal static string MakeUniqueKey(int length, string prefix)
        {
            int prefixLength = prefix == null ? 0 : prefix.Length;

            string chars = "1234567890abcdefghijklmnopqrstuvwxyz";

            StringBuilder sb = new StringBuilder();
            if (prefixLength > 0)
            {
                sb.Append(prefix);
                sb.Append("_");
                prefixLength++;
            }

            int dupCount = 0;
            int preIndex = 0;

            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < length - prefixLength; ++i)
            {
                int index = rnd.Next(0, 35);
                if (index == preIndex)
                {
                    ++dupCount;
                }
                sb.Append(chars[index]);
                preIndex = index;
            }
            if (dupCount >= length - prefixLength - 2)
            {
                rnd = new Random(Guid.NewGuid().GetHashCode());
                return MakeUniqueKey(length, prefix);
            }

            return sb.ToString();
        }

        /// <summary>
        /// ����Ƿ�Ϊ�ṹ����
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool CheckStruct(Type type)
        {
            //������Ϊ�ṹʱ����ϵ�л�
            if (type.IsValueType && !type.IsEnum && !type.IsPrimitive && !type.IsSerializable)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ת������
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static object ChangeType(object value, Type conversionType)
        {
            if (value == null) return null;

            bool isNullable = false;
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                conversionType = Nullable.GetUnderlyingType(conversionType);
                isNullable = true;
            }

            //�����ַ�������ת��
            if (value.GetType() == typeof(string))
            {
                string data = value.ToString();

                //���ת����ֵΪ�ղ��Ҷ����Ϊ��ʱ����null
                if (string.IsNullOrEmpty(data) && isNullable) return null;

                bool success;
                value = ConverterFactory.Converters[conversionType].ConvertTo(data, out success);
                if (success)
                    return value;
                else
                    throw new MySoftException(string.Format("��{0}��ת�����������͡�{1}������", value, conversionType.Name));
            }
            else
                return Convert.ChangeType(value, conversionType);
        }

        #endregion
    }
}