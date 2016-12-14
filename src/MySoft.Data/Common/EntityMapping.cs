using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MySoft.Data.Mapping
{
    /// <summary>
    /// 表映射设置
    /// </summary>
    [Serializable]
    [XmlRoot("TableSetting")]
    public class TableSetting
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        [XmlAttribute("Namespace")]
        public string Namespace { get; set; }

        /// <summary>
        /// 表前缀
        /// </summary>
        [XmlAttribute("Prefix")]
        public string Prefix { get; set; }

        /// <summary>
        /// 表后缀
        /// </summary>
        [XmlAttribute("Suffix")]
        public string Suffix { get; set; }

        /// <summary>
        /// 表映射
        /// </summary>
        [XmlElement("TableMapping")]
        public TableMapping[] Mappings { get; set; }

        /// <summary>
        /// 初始化TableSetting
        /// </summary>
        public TableSetting()
        {
            this.Mappings = new TableMapping[0];
        }
    }

    /// <summary>
    /// 表映射节点
    /// </summary>
    [Serializable]
    [XmlRoot("TableMapping")]
    public class TableMapping
    {
        /// <summary>
        /// 类名称
        /// </summary>
        [XmlAttribute("ClassName")]
        public string ClassName { get; set; }

        /// <summary>
        /// 使用前缀
        /// </summary>
        [XmlAttribute("UsePrefix")]
        public bool UsePrefix { get; set; }

        /// <summary>
        /// 使用后缀
        /// </summary>
        [XmlAttribute("UseSuffix")]
        public bool UseSuffix { get; set; }

        /// <summary>
        /// 映射的表名
        /// </summary>
        [XmlAttribute("MappingName")]
        public string MappingName { get; set; }

        /// <summary>
        /// 字段映射
        /// </summary>
        [XmlElement("FieldMapping")]
        public FieldMapping[] Mappings { get; set; }

        /// <summary>
        /// 初始化TableMapping
        /// </summary>
        public TableMapping()
        {
            this.UsePrefix = true;
            this.UseSuffix = true;
            this.Mappings = new FieldMapping[0];
        }
    }

    /// <summary>
    /// 字段映射节点
    /// </summary>
    [Serializable]
    [XmlRoot("FieldMapping")]
    public class FieldMapping
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        [XmlAttribute("PropertyName")]
        public string PropertyName { get; set; }

        /// <summary>
        /// 映射的字段名
        /// </summary>
        [XmlAttribute("MappingName")]
        public string MappingName { get; set; }
    }
}
