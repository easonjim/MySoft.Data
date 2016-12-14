using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 列表转换
    /// </summary>
    interface IListConvert<T>
    {
        /// <summary>
        /// 返回另一类型的列表
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        SourceList<TOutput> ConvertTo<TOutput>();

        /// <summary>
        /// 返回另一类型的列表(输出为接口)
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <typeparam name="IOutput"></typeparam>
        /// <returns></returns>
        SourceList<IOutput> ConvertTo<TOutput, IOutput>() where TOutput : IOutput;

        /// <summary>
        /// 将当前类型转成另一种类型
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        SourceList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter);
    }
}
