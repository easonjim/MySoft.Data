using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MySoft.Data.SqlServer;

namespace MySoft.Data.MsAccess
{
    /// <summary>
    /// Access 驱动
    /// </summary>
    public class MsAccessProvider : DbProvider
    {
        public MsAccessProvider(string connectionString)
            : base(connectionString, OleDbFactory.Instance, '[', ']', '@')
        {
        }

        /// <summary>
        /// 是否为access驱动
        /// </summary>
        protected override bool AccessProvider
        {
            get { return true; }
        }

        /// <summary>
        /// 是否支持批处理
        /// </summary>
        protected internal override bool SupportBatch
        {
            get { return false; }
        }

        /// <summary>
        /// 返回自动ID的sql语句
        /// </summary>
        protected override string RowAutoID
        {
            get { return "select max({0}) from {1}"; }
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
                foreach (OleDbParameter p in command.Parameters)
                {
                    if (p.Size > 0)
                    {
                        if (p.Scale > 0)
                            sb.Append(string.Format("{0}[{2}][{3}({4},{5})] = {1}\r\n", p.ParameterName, p.Value, p.DbType, p.OleDbType, p.Size, p.Scale));
                        else
                            sb.Append(string.Format("{0}[{2}][{3}({4})] = {1}\r\n", p.ParameterName, p.Value, p.DbType, p.OleDbType, p.Size));
                    }
                    else
                        sb.Append(string.Format("{0}[{2}][{3}] = {1}\r\n", p.ParameterName, p.Value, p.DbType, p.OleDbType));
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
            OleDbParameter p = new OleDbParameter();
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
        /// 调整命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override DbCommand PrepareCommand(DbCommand cmd)
        {
            foreach (OleDbParameter p in cmd.Parameters)
            {
                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue) continue;
                if (p.Value == DBNull.Value) continue;

                if (p.DbType == DbType.String || p.DbType == DbType.AnsiString || p.DbType == DbType.AnsiStringFixedLength)
                {
                    if (p.Size > 4000)
                    {
                        p.OleDbType = OleDbType.LongVarWChar;
                    }
                    else
                    {
                        p.OleDbType = OleDbType.VarWChar;
                    }
                }
                else if (p.DbType == DbType.Binary)
                {
                    if (p.Size > 8000)
                    {
                        p.OleDbType = OleDbType.LongVarWChar;
                    }
                }
                else if (p.DbType == DbType.Date || p.DbType == DbType.DateTime)
                {
                    p.Value = p.Value.ToString();
                }
            }

            return base.PrepareCommand(cmd);
        }
    }
}
