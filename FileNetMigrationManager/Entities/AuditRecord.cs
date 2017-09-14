using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileNetMigrationManager.Entities
{
    [Serializable]
    public class AuditRecord
    {
        [XmlElement(ElementName = "DateProcessed", DataType = "dateTime")]
        public DateTime DateProcessed { get; set; }

        [XmlElement(ElementName = "IsSuccess", DataType = "boolean")]
        public bool IsSuccess { get; set; }

        [XmlElement(ElementName = "EventMessage", DataType = "string")]
        public string EventMessage { get; set; }

        [XmlElement(ElementName = "ErrorString", DataType = "string")]
        public string ErrorString { get; set; }

        [XmlElement(ElementName = "SourceId", DataType = "string")]
        public string SourceDocId { get; set; }

        [XmlElement(ElementName = "SourceVersionId", DataType = "string")]
        public string SourceDocVersionId { get; set; }

        [XmlElement(ElementName = "DestId", DataType = "string")]
        public string DestId { get; set; }

        [XmlElement(ElementName = "DestVersionId", DataType = "string")]
        public string DestVersionId { get; set; }
    }
}
