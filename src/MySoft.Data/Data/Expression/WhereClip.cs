using System;
using System.Data.Common;
using System.Collections.Generic;

namespace MySoft.Data
{
    /// <summary>
    /// 条件构造器
    /// </summary>
    [Serializable]
    public class WhereClip
    {
        public static readonly WhereClip All = new WhereClip((string)null);
        private List<SQLParameter> plist = new List<SQLParameter>();
        private string where;

        /// <summary>
        /// 返回参数列表
        /// </summary>
        internal SQLParameter[] Parameters
        {
            get
            {
                return plist.ToArray();
            }
        }

        /// <summary>
        /// 自定义一个Where条件
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        public WhereClip(string where, params SQLParameter[] parameters)
        {
            this.where = where;
            if (parameters != null && parameters.Length > 0)
            {
                this.plist.AddRange(parameters);
            }
        }
        /// <summary>
        /// 自定义一个Where条件
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        public WhereClip(string where, IDictionary<string, object> parameters)
            : this(where)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> kv in parameters)
                {
                    this.plist.Add(new SQLParameter(kv.Key, kv.Value));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator true(WhereClip right)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator false(WhereClip right)
        {
            return false;
        }

        /// <summary>
        /// 非
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static WhereClip operator !(WhereClip where)
        {
            if (DataUtils.IsNullOrEmpty(where))
            {
                return null;
            }
            return new WhereClip(" not " + where.ToString(), where.Parameters);
        }

        /// <summary>
        /// 与
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WhereClip operator &(WhereClip leftWhere, WhereClip rightWhere)
        {
            if (DataUtils.IsNullOrEmpty(leftWhere) && DataUtils.IsNullOrEmpty(rightWhere))
            {
                return WhereClip.All;
            }
            if (DataUtils.IsNullOrEmpty(leftWhere))
            {
                return rightWhere;
            }
            if (DataUtils.IsNullOrEmpty(rightWhere))
            {
                return leftWhere;
            }

            List<SQLParameter> list = new List<SQLParameter>();
            list.AddRange(leftWhere.Parameters);
            list.AddRange(rightWhere.Parameters);

            return new WhereClip(leftWhere.ToString() + " and " + rightWhere.ToString(), list.ToArray());
        }

        /// <summary>
        /// 或者
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WhereClip operator |(WhereClip leftWhere, WhereClip rightWhere)
        {
            if (DataUtils.IsNullOrEmpty(leftWhere) && DataUtils.IsNullOrEmpty(rightWhere))
            {
                return WhereClip.All;
            }
            if (DataUtils.IsNullOrEmpty(leftWhere))
            {
                return rightWhere;
            }
            if (DataUtils.IsNullOrEmpty(rightWhere))
            {
                return leftWhere;
            }

            List<SQLParameter> list = new List<SQLParameter>();
            list.AddRange(leftWhere.Parameters);
            list.AddRange(rightWhere.Parameters);

            return new WhereClip(leftWhere.ToString() + " or " + rightWhere.ToString(), list.ToArray());
        }

        /// <summary>
        /// 添加括号
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static WhereClip Bracket(WhereClip where)
        {
            return new WhereClip("(" + where.ToString() + ")", where.Parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return where;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
