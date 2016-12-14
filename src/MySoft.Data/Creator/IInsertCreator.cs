using System;
using System.Collections.Generic;

namespace MySoft.Data
{
    interface IInsertCreator
    {
        InsertCreator AddInsert(IDictionary<Field, object> dict);
        InsertCreator AddInsert(IDictionary<string, object> dict);
        InsertCreator AddInsert(string fieldName, object value);
        InsertCreator AddInsert(Field field, object value);
        InsertCreator AddInsert(string[] fieldNames, object[] values);
        InsertCreator AddInsert(Field[] fields, object[] values);
        InsertCreator From<T>() where T : Entity;
        InsertCreator From(string tableName);
        InsertCreator From(Table table);
        InsertCreator RemoveInsert(params string[] fieldNames);
        InsertCreator RemoveInsert(params Field[] fields);
        InsertCreator SetEntity<T>(T entity) where T : Entity;
        InsertCreator SetIdentityField(string fieldName);
        InsertCreator SetIdentityField(Field field);
    }
}
