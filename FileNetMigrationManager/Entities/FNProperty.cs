using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FileNetMigrationManager
{
    [Serializable]
    /// <summary>
    /// This class for the customer FileNet property entity
    /// </summary>
    public class FNProperty : INotifyPropertyChanged
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
                OnPropertyChanged("DisplayName");
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
                OnPropertyChanged("SymbolicName");
            }
        }

        /// <summary>
        /// Property date type
        /// </summary>
        [XmlElement(ElementName = "DataType")]
        public FileNet.Api.Constants.TypeID DataType { get; set; }

        /// <summary>
        /// Is value required flag
        /// </summary>
        [XmlElement(DataType = "boolean", ElementName = "IsValueRequired", IsNullable = true)]
        public bool? IsValueRequired { get; set; }

        /// <summary>
        /// Property Cardinality (Single or Multi value)
        /// </summary>
        [XmlElement(ElementName = "Cardinality")]
        public FileNet.Api.Constants.Cardinality Cardinality { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
