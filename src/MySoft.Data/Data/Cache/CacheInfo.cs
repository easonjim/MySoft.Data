using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using System.Web;
using System.IO;

namespace MySoft.Data
{
    /// <summary>
    /// 缓存配置信息
    /// </summary>
    internal sealed class CacheConfigInfo
    {
        private string entityName;
        public string EntityName
        {
            get
            {
                return entityName;
            }
            set
            {
                entityName = value;
            }
        }

        private int expireSeconds;
        public int ExpireSeconds
        {
            get
            {
                return expireSeconds;
            }
            set
            {
                expireSeconds = value;
            }
        }

        private IList<string> relationList = new List<string>();
        public IList<string> RelationList
        {
            get
            {
                return relationList;
            }
        }
    }

    /// <summary>
    /// 缓存信息
    /// </summary>
    internal sealed class CacheInfo
    {
        private DateTime cacheTime;
        public DateTime CacheTime
        {
            get
            {
                return cacheTime;
            }
            set
            {
                cacheTime = value;
            }
        }

        private IList<string> cacheList = new List<string>();
        public IList<string> CacheList
        {
            get
            {
                return cacheList;
            }
        }
    }
}
