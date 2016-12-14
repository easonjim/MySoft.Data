using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MySoft.Data
{
    internal sealed class CacheConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 默认过期时间为60秒
        /// </summary>
        public const int DEFAULT_EXPIRE_SECONDS = 60;
        private bool? enable = null;

        [ConfigurationProperty("enable")]
        public bool Enable
        {
            get
            {
                if (this["enable"] != null && enable == null)
                {
                    enable = (bool)this["enable"];
                }

                if (enable == null)
                {
                    enable = false;
                }

                return enable.Value;
            }
            set
            {
                enable = value;
            }
        }

        [ConfigurationProperty("cacheEntities", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection CacheEntities
        {
            get
            {
                return (KeyValueConfigurationCollection)this["cacheEntities"];
            }
        }

        [ConfigurationProperty("cacheRelations", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection CacheRelations
        {
            get
            {
                return (KeyValueConfigurationCollection)this["cacheRelations"];
            }
        }
    }
}
