using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FileNetMigrationManager
{
    [Serializable]
    /// <summary>
    /// This class for the Properties mapping
    /// </summary>
    public class PropertiesMap : INotifyPropertyChanged
    {
        private FNProperty _sourceProperty;
        /// <summary>
        /// Source FileNet Property
        /// </summary>
        [XmlElement(ElementName = "SourceProperty")]
        public FNProperty SourceProperty
        {
            get { return _sourceProperty; }
            set
            {
                _sourceProperty = value;
                OnPropertyChanged("SourceProperty");
            }
        }
        
        private FNProperty _destinationProperty;
        /// <summary>
        /// Destination FileNet Property
        /// </summary>
        [XmlElement(ElementName = "DestinationProperty")]
        public FNProperty DestinationProperty
        {
            get { return _destinationProperty; }
            set
            {
                _destinationProperty = value;
                OnPropertyChanged("DestinationProperty");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
