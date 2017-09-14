using System;
using System.Xml.Serialization;

namespace FileNetMigrationManager
{
    [Serializable]
    public class FNObjStoreClass
    {
        private string _displayName;
        /// <summary>
        /// Property display name
        /// </summary>
        [XmlElement(DataType = "string", ElementName = "DisplayName")]
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
            }
        }

        private string _symbolicName;
        /// <summary>
        /// Property symbolic name
        /// </summary>
        [XmlElement(DataType = "string", ElementName = "SymbolicName")]
        public string SymbolicName
        {
            get { return _symbolicName; }
            set
            {
                _symbolicName = value;
            }
        }
    }
}
