namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal class ToString : IStringConverter
    {
        object IStringConverter.ConvertTo(string value, out bool succeeded)
        {
            succeeded = true;
            return value;
        }
    }
}

