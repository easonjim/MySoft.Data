using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
    public static class DataAccess
    {
        /// <summary>
        /// 通过配置节来实例化DbSession
        /// </summary>
        public static readonly MyDbSession DefaultSession = new MyDbSession();
    }

    /// <summary>
    /// DataExample会话类
    /// </summary>
    public class MyDbSession : DbSession
    {
        public MyDbSession(): base("ConnectionString")  // 这里是连接字符串的名字
        {
            this.RegisterSqlLogger(log =>
            {
                DebugView.PrintDebug(log);
            });
        }
    }
}
