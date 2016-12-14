using System;
using System.Collections.Generic;
using System.Text;
using MySoft.Data;
using System.Data;
using System.Text.RegularExpressions;

namespace MySoft.Data
{
    public class BaseDao<T> where T : Entity
    {
        #region 构造

        public DbSession db;
        public BaseDao(DbSession _db)
        {
            db = _db;
        }

        public BaseDao()
            : this(DataAccess.DefaultSession)
        {

        }

        #endregion

        #region 查询
        /// <summary>
        /// 通过条件得到对象
        /// </summary>
        /// <param name="where"></param>
        /// <returns>如果你的条件是得到单个 那么list《T》[0]就是这个对象</returns>
        public virtual List<T> Get_Entity_byWhere(WhereClip where, OrderByClip orderby, params Field[] fields)
        {
            return db.From<T>()
                .Where(where)
                .Select(fields)
                .OrderBy(orderby)
                .ToList();
        }

        /// <summary>
        /// 得到DataTable数据集合
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual DataTable Get_Entity_byWhere_ToTable(WhereClip where, OrderByClip orderby, params Field[] fields)
        {
            return db.From<T>()
                .Where(where)
                .Select(fields)
                .OrderBy(orderby)
                .ToTable();
        }
        /// <summary>
        /// 通过条件得到单个对象
        /// </summary>
        /// <param name="where"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual T Get_SingleEntity_byWhere(WhereClip where, params Field[] fields)
        {
            return db.From<T>()
                .Where(where)
                .Select(fields)
                .ToSingle();
        }

        /// <summary>
        /// 得到所有的数据
        /// </summary>
        /// <returns>datatable</returns>
        public virtual DataTable Get_AllData_Table()
        {
            return db.From<T>().ToTable();
        }
        /// <summary>
        /// 得到所有的数据
        /// </summary>
        /// <returns>List<T></returns>
        public virtual List<T> Get_AllData_List()
        {
            return db.From<T>().ToList();
        }
        public virtual List<T> Get_AllData_List(params Field[] fields)
        {
            return db.From<T>().Select(fields).ToList();
        }
        /// <summary>
        /// 得到top的几条数据
        /// </summary>
        /// <param name="top"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public virtual List<T> Get_Entitys_ByTop(int top, WhereClip where, OrderByClip orderby, params Field[] fields)
        {
            if (top == 0) throw new Exception("top值不能为0");
            return db.From<T>()
                .Where(where)
                .Select(fields)
                .GetTop(top)
                .OrderBy(orderby)
                .ToList();
        }
        /// <summary>
        /// 查询是否存在记录
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual bool Get_IsExist_ByWhere(WhereClip where)
        {
            return db.From<T>()
                 .Where(where)
                 .Count() > 0;
        }
        #endregion

        #region 更新
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Update_Entity(T entity)
        {
            entity.Attach();
            return db.Save<T>(entity) > 0;
        }
        /// <summary>
        /// 通过多个条件更新对象
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual bool Update_Entity_byWhere(Field[] fields, object[] values, WhereClip where)
        {
            return db.Update<T>(fields, values, where) > 0;
        }
        /// <summary>
        /// 通过单个条件更新对象
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual bool Update_Entity_byWhere(Field filed, object value, WhereClip where)
        {
            return db.Update<T>(filed, value, where) > 0;
        }

        #endregion

        #region 删除
        /// <summary>
        /// 通过对象删除记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Delete_Entity(T entity)
        {
            return db.Delete<T>(entity) > 0;
        }

        /// <summary>
        /// 通过主键ID集合删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public virtual int Delete_Entitys(List<string> idList)
        {
            return Delete_Entitys(idList.ToArray());
        }
        /// <summary>
        /// 通过主键ID集合删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public virtual int Delete_Entitys(string[] idList)
        {
            return db.Delete<T>(idList);
        }
        /// <summary>
        /// 通过条件删除对象 如果批量删除可以传入条件 where = T.ID.in(obj[])
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual bool Delete_Entity(WhereClip where)
        {
            return db.Delete<T>(where) > 0;
        }
        #endregion

        #region 新增
        /// <summary>
        /// add an new record,
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Add_Entity(T entity)
        {
            return db.Save<T>(entity) > 0;
        }

        /// <summary>
        /// 如果存在 则更新 否则 插入 注意：表必须有主键
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Insert_Or_Update(T entity) {
            return db.InsertOrUpdate(entity)>0;
        }
        #endregion

        #region 分页
        /// <summary>
        /// 分页显示数据 需要记录总数
        /// </summary>
        /// <param name="currentPageindex">当前页码</param>
        /// <param name="pageSize">pagesize</param>
        /// <param name="where">WhereClient</param>
        /// <param name="orderby">OrderByClip</param>
        /// <param name="record">总的记录数</param>
        /// <returns></returns>
        public virtual List<T> Get_Entity_byPage(int currentPageindex, int pageSize,
            WhereClip where, OrderByClip orderby, out int record, params Field[] fields)
        {
            record = Get_Entity_Record(where);
            return Get_Entity_byPage(currentPageindex, pageSize, where, orderby, fields);
        }
        /// <summary>
        /// 分页 返回Datatable
        /// </summary>
        /// <param name="currentPageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="record">总记录</param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual DataTable Get_Entity_byPage_ToTable(int currentPageindex, int pageSize,
         WhereClip where, OrderByClip orderby, out int record, params Field[] fields)
        {
            record = Get_Entity_Record(where);
            return Get_Entity_byPage_ToTable(currentPageindex, pageSize, where, orderby, fields);
        }
        /// <summary>
        /// 通过条件得到表的记录数
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual int Get_Entity_Record(WhereClip where)
        {
            return db.From<T>().Where(where).Count();
        }
        /// <summary>
        ///  分页显示数据 不需要记录总数
        /// </summary>
        /// <param name="currentPageindex">当前页码<</param>
        /// <param name="pageSize">pagesize</param>
        /// <param name="where">WhereClient</param>
        /// <param name="orderby">OrderByClip</param>
        /// <returns></returns>
        public virtual List<T> Get_Entity_byPage(int currentPageindex, int pageSize,
            WhereClip where, OrderByClip orderby, params Field[] fields)
        {
            return db.From<T>()
                   .Where(where)
                   .Select(fields)
                   .OrderBy(orderby)
                   .GetPage(pageSize)
                   .ToList(currentPageindex);
        }
        /// <summary>
        /// 分页返回Datatable
        /// </summary>
        /// <param name="currentPageindex"></param>
        /// <param name="pageSize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual DataTable Get_Entity_byPage_ToTable(int currentPageindex, int pageSize,
            WhereClip where, OrderByClip orderby, params Field[] fields)
        {
            return db.From<T>()
                   .Where(where)
                   .Select(fields)
                   .OrderBy(orderby)
                   .GetPage(pageSize)
                   .ToTable(currentPageindex);
        }
        #endregion

        #region 批量处理
        /// <summary>
        /// 带事物的批量添加
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public virtual bool BatchAdd_Entitys(List<T> entityList)
        {
            //使用事务进行批量数据插入
            using (DbTrans trans = db.BeginTrans())
            {
                try
                {
                    DbBatch batch = trans.BeginBatch(entityList.Count);
                    entityList.ForEach(item =>
                    {
                        item.Detach();
                        batch.Save(item);
                    });
                    batch.Process();

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        public virtual bool BatchUpdate_Entitys(List<T> entityList)
        {
            //使用事务进行批量数据更新
            using (DbTrans trans = db.BeginTrans())
            {
                try
                {
                    DbBatch batch = trans.BeginBatch(entityList.Count);
                    entityList.ForEach(item =>
                    {
                        item.Attach();
                        batch.Save(item);
                    });
                    batch.Process();

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }
        #endregion
    }
}
