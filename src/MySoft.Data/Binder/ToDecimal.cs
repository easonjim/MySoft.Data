namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal class ToDecimal : IStringConverter
    {
        object IStringConverter.ConvertTo(string value, out bool succeeded)
        {
            decimal num;
            succeeded = decimal.TryParse(value, out num);
            return num;
        }
    }
}

