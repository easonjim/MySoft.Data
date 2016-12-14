namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal class ToInt16 : IStringConverter
    {
        object IStringConverter.ConvertTo(string value, out bool succeeded)
        {
            short num;
            succeeded = short.TryParse(value, out num);
            return num;
        }
    }
}

