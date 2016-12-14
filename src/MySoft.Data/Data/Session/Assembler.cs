using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary> 
    /// 考虑到某些类型没有无参的构造函数，增加了描述构造信息的专门结构 
    /// </summary> 
    public class TypeConstructor
    {
        private Type type;
        private object[] constructorParameters;
        public TypeConstructor(Type type, params object[] constructorParameters)
        {
            this.type = type;
            this.constructorParameters = constructorParameters;
        }

        public TypeConstructor(Type type) : this(type, null) { }

        public Type Type
        {
            get
            {
                return type;
            }
        }

        public object[] ConstructorParameters
        {
            get
            {
                return constructorParameters;
            }
        }
    }


    /// <summary> 
    /// 管理抽象类型与实体类型的字典类型 
    /// </summary> 
    public interface ITypeMap
    {
        /// <summary> 
        /// 根据注册的目标抽象类型，返回一个实体类型及其构造参数数组 
        /// </summary> 
        /// <param name="type"></param> 
        /// <returns></returns> 
        TypeConstructor this[Type target] { get; }

        /// <summary>
        /// 注册抽象类型需要使用的实体类型 
        /// 该类型实体具有构造参数，实际的配置信息可以从外层机制获得。 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeConstructor"></param>
        void AddTypeConstructor(Type type, TypeConstructor typeConstructor);
    }


    /// <summary> 
    /// 管理抽象类型与实际实体类型映射关系，实际工程中应该从配置系统、参数系统获得。 
    /// 这里为了示例方便，采用了一个纯内存字典的方式。 
    /// </summary> 
    public class TypeMap : ITypeMap
    {
        private IDictionary<Type, TypeConstructor> dictionary = new Dictionary<Type, TypeConstructor>();
        public static readonly ITypeMap Instance;

        /// <summary> 
        /// Singleton 
        /// </summary> 
        private TypeMap() { }
        static TypeMap()
        {
            Instance = new TypeMap();
        }

        /// <summary> 
        /// 根据注册的目标抽象类型，返回一个实体类型及其构造参数数组 
        /// </summary> 
        /// <param name="type"></param> 
        /// <returns></returns> 
        public TypeConstructor this[Type type]
        {
            get
            {
                TypeConstructor result;
                if (!dictionary.TryGetValue(type, out result))
                    return null;
                else
                    return result;
            }
        }

        /// <summary>
        /// 注册抽象类型需要使用的实体类型 
        /// 该类型实体具有构造参数，实际的配置信息可以从外层机制获得。 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeConstructor"></param>
        public void AddTypeConstructor(Type type, TypeConstructor typeConstructor)
        {
            if (!dictionary.ContainsKey(type))
            {
                dictionary.Add(type, typeConstructor);
            }
            else
            {
                throw new MySoftException(string.Format("类型{0}注入重复！", type.FullName));
            }
        }
    }

    /// <summary>
    /// 提供注入的实现类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Assembler<T>
    {
        /// <summary> 
        /// 其实TypeMap工程上本身就是个需要注入的类型，可以通过访问配置系统获得， 
        /// 这里为了示例的方便，手工配置了一些类型映射信息。 
        /// </summary> 
        private static ITypeMap map = TypeMap.Instance;

        /// <summary>
        /// 创建一个实例
        /// </summary>
        /// <returns></returns>
        public T Create()
        {
            TypeConstructor constructor = map[typeof(T)];
            if (constructor != null)
            {
                if (constructor.ConstructorParameters == null)
                    return (T)Activator.CreateInstance(constructor.Type);
                else
                    return (T)Activator.CreateInstance(
                    constructor.Type, constructor.ConstructorParameters);
            }
            else
                return default(T);
        }
    }
}

