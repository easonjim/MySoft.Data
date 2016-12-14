using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    interface IPageSection<T>
    {
        int PageCount { get; }
        int RowCount { get; }

        T ToSingle(int pageIndex);

        ArrayList<object> ToListResult(int pageIndex);
        ArrayList<TResult> ToListResult<TResult>(int pageIndex);

        SourceTable ToTable(int pageIndex);
        SourceList<T> ToList(int pageIndex);

        SourceReader ToReader(int pageIndex);
    }
}
