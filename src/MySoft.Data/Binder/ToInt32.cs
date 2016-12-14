namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal class ToInt32 : IStringConverter
    {
        object IStringConverter.ConvertTo(string value, out bool succeeded)
        {
            int num;
            succeeded = int.TryParse(value, out num);
            return num;
        }
    }
}

