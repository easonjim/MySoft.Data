using System;

namespace MySoft.Data
{
    interface IQueryCreator
    {
        QueryCreator AddField(string tableName, string fieldName);
        QueryCreator AddField(Field field);
        QueryCreator AddField(string fieldName);
        QueryCreator AddField<T>(string fieldName) where T : Entity;
        QueryCreator AddOrder(Field field, bool desc);
        QueryCreator AddOrder(string orderby);
        QueryCreator AddOrder(OrderByClip order);
        QueryCreator AddOrder(string fieldName, bool desc);
        QueryCreator AddOrder(string tableName, string fieldName, bool desc);
        QueryCreator AddOrder<T>(string fieldName, bool desc) where T : Entity;
        QueryCreator AddWhere(Field field, object value);
        QueryCreator AddWhere<T>(string fieldName, object value) where T : Entity;
        QueryCreator AddWhere(string tableName, string fieldName, object value);
        QueryCreator AddWhere(string fieldName, object value);
        QueryCreator AddWhere(WhereClip where);
        QueryCreator AddWhere(string where, params SQLParameter[] parameters);
        QueryCreator From(Table table);
        QueryCreator From(string tableName);
        QueryCreator From<T>() where T : Entity;
        QueryCreator Join<T>(string where, params SQLParameter[] parameters) where T : Entity;
        QueryCreator Join(Table table, WhereClip where);
        QueryCreator Join(string tableName, string where, params SQLParameter[] parameters);
        QueryCreator Join<T>(JoinType joinType, string where, params SQLParameter[] parameters) where T : Entity;
        QueryCreator Join<T>(JoinType joinType, WhereClip where) where T : Entity;
        QueryCreator Join(JoinType joinType, Table table, WhereClip where);
        QueryCreator Join<T>(WhereClip where) where T : Entity;
        QueryCreator Join(JoinType joinType, string tableName, string where, params SQLParameter[] parameters);
        QueryCreator RemoveField(params string[] fieldNames);
        QueryCreator RemoveField(params Field[] fields);
    }
}
