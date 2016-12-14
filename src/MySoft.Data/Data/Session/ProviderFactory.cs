using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MySoft.Data
{
    /// <summary>
    /// 驱动类型
    /// </summary>
    public enum ProviderType
    {
        /// <summary>
        /// Access数据库
        /// </summary>
        [EnumDescription("MySoft.Data.Access.AccessProvider")]
        Access,
        /// <summary>
        /// SqlServer2000数据库
        /// </summary>
        [EnumDescription("MySoft.Data.SqlServer.SqlServerProvider")]
        SqlServer,
        /// <summary>
        /// SqlServer2005数据库
        /// </summary>
        [EnumDescription("MySoft.Data.SqlServer9.SqlServer9Provider")]
        SqlServer9,
        /// <summary>
        /// Oracle数据库
        /// </summary>
        [EnumDescription("MySoft.Data.Oracle.OracleProvider")]
        Oracle,
        /// <summary>
        /// MySql数据库
        /// </summary>
        [EnumDescription("MySoft.Data.MySql.MySqlProvider, MySoft.Data.MySql")]
        MySql,
        /// <summary>
        /// SQLite数据库
        /// </summary>
        [EnumDescription("MySoft.Data.SQLite.SQLiteProvider, MySoft.Data.SQLite")]
        SQLite,
        /// <summary>
        /// FireBird数据库
        /// </summary>
        [EnumDescription("MySoft.Data.FireBird.FireBirdProvider, MySoft.Data.FireBird")]
        FireBird,
        /// <summary>
        /// PostgreSQL数据库
        /// </summary>
        [EnumDescription("MySoft.Data.PostgreSQL.PostgreSQLProvider, MySoft.Data.PostgreSQL")]
        PostgreSQL
    }

    /// <summary>
    /// The db provider factory.
    /// </summary>
    public sealed class ProviderFactory
    {
        #region Private Members

        private static IDictionary<string, DbProvider> providerCache = new Dictionary<string, DbProvider>();

        private ProviderFactory() { }

        #endregion

        #region Public Members

        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="connectionString">Name of the conn STR.</param>
        /// <returns>The db provider.</returns>
        public static DbProvider CreateDbProvider(ProviderType providerType, string connectionString)
        {
            try
            {
                string[] assAndClass = EnumDescriptionAttribute.GetDescription(providerType).Split(',');
                DbProvider dbProvider;
                if (assAndClass.Length > 1)
                    dbProvider = CreateDbProvider(assAndClass[1].Trim(), assAndClass[0].Trim(), connectionString);
                else
                    dbProvider = CreateDbProvider(null, assAndClass[0].Trim(), connectionString);

                if (dbProvider != null) dbProvider.ConnectName = "Default";
                return dbProvider;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="connectionString">The conn STR.</param>
        /// <returns>The db provider.</returns>
        public static DbProvider CreateDbProvider(string assemblyName, string className, string connectionString)
        {
            //Check.Require(!string.IsNullOrEmpty(className), "className could not be null.");

            if (connectionString.ToLower().Contains("microsoft.jet.oledb") || connectionString.ToLower().Contains(".db3"))
            {
                if (connectionString.ToLower().IndexOf("data source") < 0)
                {
                    throw new MySoftException("ConnectionString的格式有错误，请查证！");
                }
                string mdbPath = connectionString.Substring(connectionString.ToLower().IndexOf("data source") + "data source".Length + 1).TrimStart(' ', '=');
                if (mdbPath.ToLower().StartsWith("|datadirectory|"))
                {
                    mdbPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\App_Data" + mdbPath.Substring("|datadirectory|".Length);
                }
                else if (connectionString.StartsWith("./") || connectionString.EndsWith(".\\"))
                {
                    connectionString = connectionString.Replace("/", "\\").Replace(".\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                }
                connectionString = connectionString.Substring(0, connectionString.ToLower().IndexOf("data source")) + "Data Source=" + mdbPath;
            }

            //如果是~则表示当前目录
            if (connectionString.Contains("~/") || connectionString.Contains("~\\"))
            {
                connectionString = connectionString.Replace("/", "\\").Replace("~\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
            }

            //by default, using sqlserver db provider
            if (string.IsNullOrEmpty(className))
            {
                className = typeof(SqlServer.SqlServerProvider).ToString();
            }
            else if (className.ToLower().IndexOf("System.Data.SqlClient".ToLower()) >= 0 || className.ToLower().Trim() == "sql" || className.ToLower().Trim() == "sqlserver")
            {
                className = typeof(SqlServer.SqlServerProvider).ToString();
            }
            else if (className.ToLower().Trim() == "sql9" || className.ToLower().Trim() == "sqlserver9" || className.ToLower().Trim() == "sqlserver2005" || className.ToLower().Trim() == "sql2005")
            {
                className = typeof(SqlServer9.SqlServer9Provider).ToString();
            }

            System.Reflection.Assembly ass;
            if (string.IsNullOrEmpty(assemblyName))
                ass = typeof(DbProvider).Assembly;
            else
                ass = System.Reflection.Assembly.Load(assemblyName);

            string cacheKey = string.Format("{0}_{1}_{2}", assemblyName, className, connectionString);
            if (providerCache.ContainsKey(cacheKey))
            {
                return providerCache[cacheKey];
            }
            else
            {
                DbProvider retProvider = ass.CreateInstance(className, false, System.Reflection.BindingFlags.Default, null, new object[] { connectionString }, null, null) as DbProvider;
                if (retProvider != null) providerCache[cacheKey] = retProvider;
                return retProvider;
            }
        }

        /// <summary>
        /// Gets the default db provider.
        /// </summary>
        /// <value>The default.</value>
        public static DbProvider Default
        {
            get
            {
                try
                {
                    DbProvider dbProvider;
                    ConnectionStringSettings connStrSetting = ConfigurationManager.ConnectionStrings[ConfigurationManager.ConnectionStrings.Count - 1];
                    string[] assAndClass = connStrSetting.ProviderName.Split(',');
                    if (assAndClass.Length > 1)
                    {
                        dbProvider = CreateDbProvider(assAndClass[1].Trim(), assAndClass[0].Trim(), connStrSetting.ConnectionString);
                    }
                    else
                    {
                        dbProvider = CreateDbProvider(null, assAndClass[0].Trim(), connStrSetting.ConnectionString);
                    }
                    if (dbProvider != null) dbProvider.ConnectName = connStrSetting.Name;
                    return dbProvider;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="connectName">Name of the conn STR.</param>
        /// <returns>The db provider.</returns>
        public static DbProvider CreateDbProvider(string connectName)
        {
            try
            {
                DbProvider dbProvider;
                ConnectionStringSettings connStrSetting = ConfigurationManager.ConnectionStrings[connectName];
                string[] assAndClass = connStrSetting.ProviderName.Split(',');
                if (assAndClass.Length > 1)
                {
                    dbProvider = CreateDbProvider(assAndClass[1].Trim(), assAndClass[0].Trim(), connStrSetting.ConnectionString);
                }
                else
                {
                    dbProvider = CreateDbProvider(null, assAndClass[0].Trim(), connStrSetting.ConnectionString);
                }
                dbProvider.ConnectName = connectName;
                return dbProvider;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="connStrName">Name of the conn STR.</param>
        /// <returns>The db provider.</returns>
        public static DbProvider CreateDbProvider(string connectName, ProviderType providerType)
        {
            try
            {
                ConnectionStringSettings connStrSetting = ConfigurationManager.ConnectionStrings[connectName];
                DbProvider dbProvider = CreateDbProvider(providerType, connStrSetting.ConnectionString);
                if (dbProvider != null) dbProvider.ConnectName = connStrSetting.Name;

                return dbProvider;
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
