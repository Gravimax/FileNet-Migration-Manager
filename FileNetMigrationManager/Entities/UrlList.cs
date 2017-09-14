using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileNetMigrationManager
{
    [Serializable]
    public class UrlList
    {
        [XmlArray(ElementName = "MtomUrls", IsNullable = true)]
        public List<string> MtomUrls { get; set; }
    }
}
