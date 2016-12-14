namespace MySoft.Data
{
    using System;
    using System.Runtime.InteropServices;

    internal class ToGuid : IStringConverter
    {
        object IStringConverter.ConvertTo(string value, out bool succeeded)
        {
            Guid guid;
            try
            {
                guid = new Guid(value);
                succeeded = true;
            }
            catch
            {
                guid = Guid.Empty;
                succeeded = false;
            }
            return guid;
        }
    }
}

