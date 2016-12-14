using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MySoft.Data.SqlServer;
using MySoft.Data.Design;

namespace MySoft.Data.SqlServer9
{
    /// <summary>
    /// SQL Server 2005 驱动
    /// </summary>
    public class SqlServer9Provider : SqlServerProvider
    {
        public SqlServer9Provider(string connectionString)
            : base(connectionString)
        { }

        /// <summary>
        /// 创建分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        protected internal override QuerySection<T> CreatePageQuery<T>(QuerySection<T> query, int itemCount, int skipCount)
        {
            if (skipCount == 0)
            {
                ((IPaging)query).Prefix("top " + itemCount);
                return query;
            }
            else
            {
                //如果没有指定Order 则由指定的key来排序
                if (query.OrderString == null)
                {
                    Field pagingField = query.PagingField;

                    if ((IField)pagingField == null)
                    {
                        throw new MySoftException("请指定分页主键或设置排序！");
                    }

                    query.OrderBy(pagingField.Asc);
                }

                ((IPaging)query).Suffix(",row_number() over(" + query.OrderString + ") as tmp__rowid");
                query.OrderBy(OrderByClip.Default);

                QuerySection<T> jquery = query.SubQuery("tmp_table");
                jquery.Where(new WhereClip("tmp__rowid between " + (skipCount + 1) + " and " + (itemCount + skipCount)));
                jquery.Select(Field.All.At("tmp_table"));

                return jquery;
            }
        }
    }
}
