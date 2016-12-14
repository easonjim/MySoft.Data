using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MySoft.Data.Mvp
{
    public sealed class ServiceConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("services", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceConfigurationElementCollection))]
        public ServiceConfigurationElementCollection Services
        {
            get
            {
                return (ServiceConfigurationElementCollection)base["services"];
            }
        }
    }

    public sealed class ServiceConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("service", IsRequired = true)]
        public string Service
        {
            get { return (string)this["service"]; }
            set { this["service"] = value; }
        }
    }

    public sealed class ServiceConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceConfigurationElement)element).Key;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public ServiceConfigurationElement this[int index]
        {
            get { return BaseGet(index) as ServiceConfigurationElement; }
        }

        public new ServiceConfigurationElement this[string key]
        {
            get { return BaseGet(key) as ServiceConfigurationElement; }
        }
    }
}
