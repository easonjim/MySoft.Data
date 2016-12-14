using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace MySoft.Data
{
    /// <summary>
    /// The serialization manager.
    /// </summary>
    public sealed class SerializationManager
    {
        #region Nested Types

        /// <summary>
        /// The serialize delegate.
        /// </summary>
        /// <param name="obj">obj to be serialized.</param>
        /// <returns></returns>
        public delegate string TypeSerializeHandler(object obj);
        /// <summary>
        /// The deserialize delegate.
        /// </summary>
        /// <param name="data">the data to be deserialied.</param>
        /// <returns></returns>
        public delegate object TypeDeserializeHandler(string data);

        #endregion

        private static IDictionary<Type, KeyValuePair<TypeSerializeHandler, TypeDeserializeHandler>> handlers = new Dictionary<Type, KeyValuePair<TypeSerializeHandler, TypeDeserializeHandler>>();

        static SerializationManager()
        {
            InitDefaultSerializeHandlers();
        }

        /// <summary>
        /// Serializes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            return Serialize(obj, Encoding.UTF8);
        }

        /// <summary>
        /// Serializes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string Serialize(object obj, Encoding encoding)
        {
            if (obj == null)
            {
                return null;
            }

            if (handlers.ContainsKey(obj.GetType()))
            {
                return handlers[obj.GetType()].Key(obj);
            }
            else
            {
                MemoryStream ms = new MemoryStream();
                XmlTextWriter xw = new XmlTextWriter(ms, encoding);
                xw.Formatting = Formatting.Indented;
                xw.WriteStartDocument();
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(xw, obj);
                xw.Close();
                return encoding.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Deserializes the specified return type.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string data)
        {
            return (T)Deserialize(typeof(T), data);
        }

        /// <summary>
        /// Deserializes the specified return type.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static object Deserialize(Type returnType, string data)
        {
            if (data == null)
            {
                return null;
            }

            if (handlers.ContainsKey(returnType))
            {
                return handlers[returnType].Value(data);
            }
            else
            {
                StringReader sr = new StringReader(data);
                XmlSerializer serializer = new XmlSerializer(returnType);
                object obj = serializer.Deserialize(sr);
                sr.Close();
                return obj;
            }
        }

        /// <summary>
        /// Registers the serialize handler.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializeHandler">The serialize handler.</param>
        /// <param name="deserializeHandler">The deserialize handler.</param>
        public static void RegisterSerializeHandler(Type type, TypeSerializeHandler serializeHandler, TypeDeserializeHandler deserializeHandler)
        {
            lock (handlers)
            {
                if (handlers.ContainsKey(type))
                {
                    handlers[type] = new KeyValuePair<TypeSerializeHandler, TypeDeserializeHandler>(serializeHandler, deserializeHandler);
                }
                else
                {
                    handlers.Add(type, new KeyValuePair<TypeSerializeHandler, TypeDeserializeHandler>(serializeHandler, deserializeHandler));
                }
            }
        }

        /// <summary>
        /// Unregisters the serialize handler.
        /// </summary>
        /// <param name="type">The type.</param>
        public static void UnregisterSerializeHandler(Type type)
        {
            lock (handlers)
            {
                if (handlers.ContainsKey(type))
                {
                    handlers.Remove(type);
                }
            }
        }

        #region InitDefaultSerializeHandlers

        private static void InitDefaultSerializeHandlers()
        {
            RegisterSerializeHandler(typeof(string), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadString));
            RegisterSerializeHandler(typeof(int), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadInt));
            RegisterSerializeHandler(typeof(long), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadLong));
            RegisterSerializeHandler(typeof(short), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadShort));
            RegisterSerializeHandler(typeof(byte), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadByte));
            RegisterSerializeHandler(typeof(bool), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadBool));
            RegisterSerializeHandler(typeof(decimal), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadDecimal));
            RegisterSerializeHandler(typeof(char), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadChar));
            RegisterSerializeHandler(typeof(sbyte), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadSbyte));
            RegisterSerializeHandler(typeof(float), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadFloat));
            RegisterSerializeHandler(typeof(double), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadDouble));
            RegisterSerializeHandler(typeof(byte[]), new TypeSerializeHandler(ByteArrayToString), new TypeDeserializeHandler(LoadByteArray));
            RegisterSerializeHandler(typeof(Guid), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadGuid));
            RegisterSerializeHandler(typeof(DateTime), new TypeSerializeHandler(ToString), new TypeDeserializeHandler(LoadDateTime));
        }

        private static string ToString(object obj)
        {
            return obj.ToString();
        }

        private static object LoadString(string data)
        {
            return data;
        }

        private static object LoadInt(string data)
        {
            return int.Parse(data);
        }

        private static object LoadLong(string data)
        {
            return long.Parse(data);
        }

        private static object LoadShort(string data)
        {
            return short.Parse(data);
        }

        private static object LoadByte(string data)
        {
            return byte.Parse(data);
        }

        private static object LoadBool(string data)
        {
            return bool.Parse(data);
        }

        private static object LoadDecimal(string data)
        {
            return decimal.Parse(data);
        }

        private static object LoadChar(string data)
        {
            return char.Parse(data);
        }

        private static object LoadSbyte(string data)
        {
            return sbyte.Parse(data);
        }

        private static object LoadFloat(string data)
        {
            return float.Parse(data);
        }

        private static object LoadDouble(string data)
        {
            return double.Parse(data);
        }

        private static string ByteArrayToString(object obj)
        {
            return Convert.ToBase64String((byte[])obj);
        }

        private static object LoadByteArray(string data)
        {
            return Convert.FromBase64String(data);
        }

        private static object LoadGuid(string data)
        {
            return new Guid(data);
        }

        private static object LoadDateTime(string data)
        {
            return DateTime.Parse(data);
        }

        #endregion
    }
}
