using System;

namespace MySoft.Data
{
    interface IDeleteCreator
    {
        DeleteCreator AddWhere(string fieldName, object value);
        DeleteCreator AddWhere(string where, params SQLParameter[] parameters);
        DeleteCreator AddWhere(Field field, object value);
        DeleteCreator AddWhere(WhereClip where);
        DeleteCreator From<T>() where T : Entity;
        DeleteCreator From(Table table);
        DeleteCreator From(string tableName);
    }
}
