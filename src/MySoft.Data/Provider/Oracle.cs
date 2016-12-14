using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using MySoft.Data.Design;

namespace MySoft.Data.Oracle
{
    /// <summary>
    /// Oracle 驱动
    /// </summary>
    public class OracleProvider : DbProvider
    {
        public OracleProvider(string connectionString)
            : base(connectionString, OracleClientFactory.Instance, '"', '"', ':')
        {
        }

        /// <summary>
        /// 是否支持批处理
        /// </summary>
        protected internal override bool SupportBatch
        {
            get { return true; }
        }

        /// <summary>
        /// 是否使用自增列
        /// </summary>
        protected override bool UseAutoIncrement
        {
            get { return true; }
        }

        /// <summary>
        /// 返回自动ID的sql语句
        /// </summary>
        protected override string RowAutoID
        {
            get
            {
                return "select {0}.currval from dual";
            }
        }

        /// <summary>
        /// 格式化IdentityName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override string FormatIdentityName(string name)
        {
            return string.Format("{0}.nextval", name);
        }

        /// <summary>
        /// 获取输出日志
        /// </summary>
        /// <param name="command">The command.</param>
        protected override string GetLog(DbCommand command)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(string.Format("{0}\t{1}\t\r\n", command.CommandType, command.CommandText));
            if (command.Parameters != null && command.Parameters.Count > 0)
            {
                sb.Append("Parameters:\r\n");
                foreach (OracleParameter p in command.Parameters)
                {
                    if (p.Size > 0)
                        sb.Append(string.Format("{0}[{2}][{3}({4})] = {1}\r\n", p.ParameterName, p.Value, p.DbType, p.OracleType, p.Size));
                    else
                        sb.Append(string.Format("{0}[{2}][{3}] = {1}\r\n", p.ParameterName, p.Value, p.DbType, p.OracleType));
                }
            }
            sb.Append("\r\n");

            return sb.ToString();
        }

        /// <summary>
        /// 创建DbParameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        protected override DbParameter CreateParameter(string parameterName, object val)
        {
            OracleParameter p = new OracleParameter();
            p.ParameterName = parameterName;
            if (val == null || val == DBNull.Value)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                if (val.GetType().IsEnum)
                {
                    p.Value = Convert.ToInt32(val);
                }
                else
                {
                    p.Value = val;
                }
            }
            return p;
        }

        /// <summary>
        /// 调整DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override DbCommand PrepareCommand(DbCommand cmd)
        {
            //将sql转换成大写
            cmd.CommandText = cmd.CommandText.ToUpper();

            foreach (OracleParameter p in cmd.Parameters)
            {
                p.ParameterName = p.ParameterName.ToUpper();
                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue) continue;
                if (p.Value == DBNull.Value) continue;

                if (p.DbType == DbType.Guid)
                {
                    p.OracleType = OracleType.Char;
                    p.Size = 36;
                    p.Value = p.Value.ToString();
                }
                else if (p.DbType == DbType.String || p.DbType == DbType.AnsiString || p.DbType == DbType.AnsiStringFixedLength)
                {
                    if (p.Size > 4000)
                    {
                        p.OracleType = OracleType.NClob;
                    }
                    else
                    {
                        p.OracleType = OracleType.NVarChar;
                    }
                }
                else if (p.DbType == DbType.Binary)
                {
                    if (p.Size > 8000)
                    {
                        p.OracleType = OracleType.BFile;
                    }
                }
                else if (p.DbType == DbType.Boolean)
                {
                    p.OracleType = OracleType.Number;
                    p.Size = 1;
                    p.Value = Convert.ToBoolean(p.Value) ? 1 : 0;
                }
            }

            return base.PrepareCommand(cmd);
        }

        /// <summary>
        /// 创建分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="itemCount"></param>
        /// <param name="skipCount"></param>
        /// <returns></returns>
        protected internal override QuerySection<T> CreatePageQuery<T>(QuerySection<T> query, int itemCount, int skipCount)
        {
            if (skipCount == 0)
            {
                if (itemCount == 1 && query.OrderString == null)
                {
                    query.PageWhere = new WhereClip("rownum <= 1");
                    return query;
                }
                else
                {
                    QuerySection<T> jquery = query.SubQuery("tmp_table");
                    jquery.Where(new WhereClip("rownum <= " + itemCount));
                    jquery.Select(Field.All.At("tmp_table"));

                    return jquery;
                }
            }
            else
            {
                //如果没有指定Order 则由指定的key来排序
                if (query.OrderString == null)
                {
                    Field pagingField = query.PagingField;

                    if ((IField)pagingField == null)
                    {
                        throw new MySoftException("请设置当前实体主键字段或指定排序！");
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
