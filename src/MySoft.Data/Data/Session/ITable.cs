using System;

namespace MySoft.Data
{
    interface ITable
    {
        string OriginalName { get; }
        string Prefix { set; }
        string Suffix { set; }
        Field this[string fieldName] { get; }
    }
}
