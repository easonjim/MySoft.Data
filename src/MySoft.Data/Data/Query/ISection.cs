using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace MySoft.Data
{
    interface ISqlSection
    {
        int Execute();
        T ToSingle<T>() where T : Entity;

        SourceList<T> ToList<T>() where T : Entity;
        ArrayList<TResult> ToListResult<TResult>();

        SourceReader ToReader();
        SourceTable ToTable();
        DataSet ToDataSet();

        TResult ToScalar<TResult>();
        object ToScalar();
    }

    interface IProcSection : ISqlSection
    {
        int Execute(out IDictionary<string, object> outValues);
        T ToSingle<T>(out IDictionary<string, object> outValues) where T : Entity;

        SourceList<T> ToList<T>(out IDictionary<string, object> outValues) where T : Entity;
        ArrayList<TResult> ToListResult<TResult>(out IDictionary<string, object> outValues);

        SourceReader ToReader(out IDictionary<string, object> outValues);
        SourceTable ToTable(out IDictionary<string, object> outValues);
        DataSet ToDataSet(out IDictionary<string, object> outValues);

        TResult ToScalar<TResult>(out IDictionary<string, object> outValues);
        object ToScalar(out IDictionary<string, object> outValues);
    }
}
