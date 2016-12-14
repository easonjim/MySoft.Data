namespace MySoft.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    internal static class ConverterFactory
    {
        private static IDictionary<Type, IStringConverter> mConverters;

        private static void OnInit()
        {
            if (mConverters == null)
            {
                mConverters = new Dictionary<Type, IStringConverter>();
                mConverters.Add(typeof(byte), new ToByte());
                mConverters.Add(typeof(short), new ToInt16());
                mConverters.Add(typeof(int), new ToInt32());
                mConverters.Add(typeof(long), new ToLong());
                mConverters.Add(typeof(char), new ToChar());
                mConverters.Add(typeof(Guid), new ToGuid());
                mConverters.Add(typeof(DateTime), new ToDateTime());
                mConverters.Add(typeof(decimal), new ToDecimal());
                mConverters.Add(typeof(float), new ToFloat());
                mConverters.Add(typeof(double), new ToDouble());
                mConverters.Add(typeof(string), new ToString());
                mConverters.Add(typeof(bool), new ToBool());
            }
        }

        public static IDictionary<Type, IStringConverter> Converters
        {
            get
            {
                if (mConverters == null)
                {
                    lock (typeof(ConverterFactory))
                    {
                        OnInit();
                    }
                }
                return mConverters;
            }
        }
    }
}

