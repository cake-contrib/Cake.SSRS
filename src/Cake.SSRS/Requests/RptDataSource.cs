using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Cake.SSRS
{
    [XmlRoot(ElementName = "RptDataSource")]
    public class RptDataSource
    {
        [XmlElement(ElementName = "ConnectionProperties")]
        public ConnectionProperties ConnectionProperties { get; set; }
        [XmlElement(ElementName = "DataSourceID")]
        public string DataSourceID { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "ConnectionProperties")]
    public class ConnectionProperties
    {
        [XmlElement(ElementName = "Extension")]
        public string Extension { get; set; }
        [XmlElement(ElementName = "ConnectString")]
        public string ConnectString { get; set; }
        [XmlElement(ElementName = "IntegratedSecurity", IsNullable = true, DataType = "boolean")]
        public bool? IntegratedSecurity { get; set; }
    }
}
