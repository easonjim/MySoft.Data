namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal class ToChar : IStringConverter
    {
        public object ConvertTo(string value, out bool succeeded)
        {
            char ch;
            succeeded = char.TryParse(value, out ch);
            return ch;
        }
    }
}