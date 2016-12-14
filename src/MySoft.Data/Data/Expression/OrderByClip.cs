using System;

namespace MySoft.Data
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OrderByClip
    {
        public static readonly OrderByClip Default = new OrderByClip((string)null);
        private string orderBy;

        /// <summary>
        /// 自定义一个OrderBy条件
        /// </summary>
        /// <param name="orderBy"></param>
        public OrderByClip(string orderBy)
        {
            this.orderBy = orderBy;
        }

        public static bool operator true(OrderByClip right)
        {
            return false;
        }

        public static bool operator false(OrderByClip right)
        {
            return false;
        }

        public static OrderByClip operator &(OrderByClip leftOrder, OrderByClip rightOrder)
        {
            if (DataUtils.IsNullOrEmpty(leftOrder) && DataUtils.IsNullOrEmpty(rightOrder))
            {
                return OrderByClip.Default;
            }
            if (DataUtils.IsNullOrEmpty(leftOrder))
            {
                return rightOrder;
            }
            if (DataUtils.IsNullOrEmpty(rightOrder))
            {
                return leftOrder;
            }
            return new OrderByClip(leftOrder.ToString() + "," + rightOrder.ToString());
        }

        public override string ToString()
        {
            return orderBy;
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
