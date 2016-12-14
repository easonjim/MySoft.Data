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
    /// 缓存相关的操作类
    /// </summary>
    internal sealed class DataCache
    {
        private Hashtable inMemoryCache = Hashtable.Synchronized(new Hashtable());
        private IDictionary<string, CacheInfo> cacheMap = new Dictionary<string, CacheInfo>();
        internal IDictionary<string, CacheInfo> CacheMap
        {
            get
            {
                return cacheMap;
            }
        }

        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        internal object GetCache(string cacheKey)
        {
            lock (inMemoryCache.SyncRoot)
            {
                if (inMemoryCache.ContainsKey(cacheKey))
                {
                    return inMemoryCache[cacheKey];
                }
                return null;
            }
        }

        internal void RemoveCache(CacheConfigInfo cacheConfigInfo)
        {
            lock (cacheConfigInfo)
            {
                //移除有关联关系表中的缓存
                foreach (string entityName in cacheConfigInfo.RelationList)
                {
                    RemoveCache(entityName);
                }

                //移除本表中的缓存
                RemoveCache(cacheConfigInfo.EntityName);
            }
        }

        internal void RemoveCache(string entityName)
        {
            lock (cacheMap)
            {
                if (cacheMap.ContainsKey(entityName))
                {
                    IList<string> list = cacheMap[entityName].CacheList;
                    if (list.Count == 0) return;
                    foreach (string key in list)
                    {
                        RemoveKeyCache(key);
                    }
                    list.Clear();
                }
            }
        }

        private void RemoveKeyCache(string cacheKey)
        {
            lock (inMemoryCache.SyncRoot)
            {
                if (inMemoryCache.ContainsKey(cacheKey))
                {
                    inMemoryCache.Remove(cacheKey);
                }
            }
        }

        public void RemoveAllCache()
        {
            lock (inMemoryCache.SyncRoot)
            {
                inMemoryCache.Clear();
                cacheMap.Clear();
            }
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="obj"></param>
        internal void SetCache(string cacheKey, object obj)
        {
            lock (inMemoryCache.SyncRoot)
            {
                if (!inMemoryCache.ContainsKey(cacheKey))
                {
                    inMemoryCache.Add(cacheKey, obj);
                }
                else
                {
                    inMemoryCache[cacheKey] = obj;
                }
            }
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="obj"></param>
        internal void SetCache(string entityName, string cacheKey, object obj)
        {
            lock (cacheMap)
            {
                SetCache(cacheKey, obj);
                CacheInfo cacheInfo = new CacheInfo();
                if (cacheMap.ContainsKey(entityName))
                {
                    cacheInfo = cacheMap[entityName];
                    cacheInfo.CacheTime = DateTime.Now;
                    cacheInfo.CacheList.Add(cacheKey);
                    cacheMap[entityName] = cacheInfo;
                }
                else
                {
                    cacheInfo.CacheTime = DateTime.Now;
                    cacheInfo.CacheList.Add(cacheKey);
                    cacheMap.Add(entityName, cacheInfo);
                }
            }
        }
    }
}