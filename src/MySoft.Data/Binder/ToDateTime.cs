namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal class ToDateTime : IStringConverter
    {
        object IStringConverter.ConvertTo(string value, out bool succeeded)
        {
            DateTime time;
            succeeded = DateTime.TryParse(value, out time);
            return time;
        }
    }
}

