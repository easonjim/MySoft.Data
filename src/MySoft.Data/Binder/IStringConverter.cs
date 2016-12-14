namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal interface IStringConverter
    {
        object ConvertTo(string value, out bool succeeded);
    }
}

