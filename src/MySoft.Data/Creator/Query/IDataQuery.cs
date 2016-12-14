using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySoft.Data
{
    interface IUserQuery
    {
        QuerySection SetPagingField(string pagingFieldName);
        QuerySection SetPagingField(Field pagingField);

        PageSection GetPage(int pageSize);
        QuerySection<T> ToQuery<T>() where T : Entity;

        object ToScalar();
        TResult ToScalar<TResult>();
        int Count();
        bool Exists();

        T ToSingle<T>() where T : Entity;

        SourceReader ToReader();
        SourceReader ToReader(int topSize);

        SourceList<T> ToList<T>() where T : Entity;
        SourceList<T> ToList<T>(int topSize) where T : Entity;

        SourceTable ToTable();
        SourceTable ToTable(int topSize);

        DataPage<IList<T>> ToListPage<T>(int pageSize, int pageIndex) where T : Entity;
        DataPage<DataTable> ToTablePage(int pageSize, int pageIndex);
    }

    interface IPageQuery
    {
        int PageCount { get; }
        int RowCount { get; }

        SourceReader ToReader(int pageIndex);
        SourceList<T> ToList<T>(int pageIndex) where T : Entity;
        SourceTable ToTable(int pageIndex);
    }
}
