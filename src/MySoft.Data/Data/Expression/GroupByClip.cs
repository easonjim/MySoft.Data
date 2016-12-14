using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GroupByClip
    {
        public static readonly GroupByClip None = new GroupByClip((string)null);
        private string groupBy;

        /// <summary>
        /// 自定义一个GroupBy条件
        /// </summary>
        /// <param name="groupBy"></param>
        public GroupByClip(string groupBy)
        {
            this.groupBy = groupBy;
        }

        /// <summary>
        /// 自定义一个GroupBy条件
        /// </summary>
        /// <param name="fields"></param>
        public GroupByClip(params Field[] fields)
        {
            if (fields != null && fields.Length > 0)
            {
                GroupByClip group = GroupByClip.None;
                foreach (Field field in fields)
                {
                    group &= field.Group;
                }

                this.groupBy = group.ToString();
            }
        }

        public static GroupByClip operator &(GroupByClip leftGroup, GroupByClip rightGroup)
        {
            if (DataUtils.IsNullOrEmpty(leftGroup) && DataUtils.IsNullOrEmpty(rightGroup))
            {
                return GroupByClip.None;
            }
            if (DataUtils.IsNullOrEmpty(leftGroup))
            {
                return rightGroup;
            }
            if (DataUtils.IsNullOrEmpty(rightGroup))
            {
                return leftGroup;
            }
            return new GroupByClip(leftGroup.ToString() + "," + rightGroup.ToString());
        }

        public static bool operator true(GroupByClip right)
        {
            return false;
        }

        public static bool operator false(GroupByClip right)
        {
            return false;
        }

        public override string ToString()
        {
            return groupBy;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
