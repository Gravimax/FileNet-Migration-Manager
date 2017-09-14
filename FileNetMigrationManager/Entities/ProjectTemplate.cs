using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FileNetMigrationManager
{
    [Serializable]
    public class ProjectTemplate : INotifyPropertyChanged
    {
        private bool _autoCreateFolders;
        [XmlElement(DataType = "boolean", ElementName = "AutoCreateFolders")]
        public bool AutoCreateFolders
        {
            get { return _autoCreateFolders; }
            set
            {
                _autoCreateFolders = value;
                OnPropertyChanged("AutoCreateFolders");
            }
        }

        private string _destSecPassword;
        [XmlElement(DataType = "string", ElementName = "DestSecPassword")]
        public string DestSecPassword
        {
            get { return _destSecPassword; }
            set { _destSecPassword = value; }
        }

        public string _destPassword;
        [XmlIgnore]
        public string DestPassword
        {
            get { return EncryptionUtilities.StringCipher.Decrypt(_destSecPassword); }
            set
            {
                _destSecPassword = EncryptionUtilities.StringCipher.Encrypt(value);
                OnPropertyChanged("DestPassword");
            }
        }

        private string _destinationUrl;
        [XmlElement(DataType = "string", ElementName = "DestinationUrl")]
        public string DestinationUrl
        {
            get
            {
                return _destinationUrl;
            }

            set
            {
                _destinationUrl = value;
                OnPropertyChanged("DestinationUrl");
            }
        }

        private string _destUserName;
        [XmlElement(DataType = "string", ElementName = "DestUserName")]
        public string DestUserName
        {
            get
            {
                return _destUserName;
            }

            set
            {
                _destUserName = value;
                OnPropertyChanged("DestUserName");
            }
        }

        private DateTime? _fromDate;
        [XmlElement(DataType = "dateTime", ElementName = "FromDate", IsNullable = true)]
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                OnPropertyChanged("FromDate");
            }
        }

        private ObservableCollection<PropertiesMap> _propertiesMapping;
        /// <summary> List of the properties. </summary>
        [XmlArray(ElementName = "PropertiesMapping")]
        public ObservableCollection<PropertiesMap> PropertiesMapping
        {
            get { return _propertiesMapping; }
            set
            {
                _propertiesMapping = value;
                OnPropertyChanged("PropertiesMapping");
            }
        }

        private int _isAllVersions;
        /// <summary>Flag to run with all the document versions </summary>
        [XmlElement(DataType = "int", ElementName = "IsAllVersions")]
        public int IsAllVersions
        {
            get
            {
                return _isAllVersions;
            }

            set
            {
                _isAllVersions = value;
                OnPropertyChanged("IsAllVersions");
            }
        }

        private int _isMove;
        /// <summary>Flag to Move the document</summary>
        [XmlElement(DataType = "int", ElementName = "IsMove")]
        public int IsMove
        {
            get { return _isMove; }

            set
            {
                _isMove = value;
                OnPropertyChanged("IsMove");
            }
        }

        private string _srcSecPassword;
        [XmlElement(DataType = "string", ElementName = "SrcSecPassword")]
        public string SrcSecPassword
        {
            get { return _srcSecPassword; }
            set { _srcSecPassword = value; }
        }

        public string _srcPassword;
        [XmlIgnore]
        public string SrcPassword
        {
            get { return EncryptionUtilities.StringCipher.Decrypt(_srcSecPassword); }
            set
            {
                _srcSecPassword = EncryptionUtilities.StringCipher.Encrypt(value);
                OnPropertyChanged("SrcPassword");
            }
        }

        private string _sourceUrl;
        [XmlElement(DataType = "string", ElementName = "SourceUrl")]
        public string SourceUrl
        {
            get
            {
                return _sourceUrl;
            }

            set
            {
                _sourceUrl = value;
                OnPropertyChanged("SourceUrl");
            }
        }

        private string _srcUserName;
        [XmlElement(DataType = "string", ElementName = "SrcUserName")]
        public string SrcUserName
        {
            get
            {
                return _srcUserName;
            }

            set
            {
                _srcUserName = value;
                OnPropertyChanged("SrcUserName");
            }
        }

        private string _templateName;
        [XmlElement(DataType = "string", ElementName = "TemplateName")]
        public string TemplateName
        {
            get { return _templateName; }
            set
            {
                _templateName = value;
                OnPropertyChanged("TemplateName");
            }
        }

        private DateTime? _toDate;
        [XmlElement(DataType = "dateTime", ElementName = "ToDate", IsNullable = true)]
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                OnPropertyChanged("ToDate");
            }
        }

        private bool _useEditedFields;
        [XmlElement(DataType = "boolean", ElementName = "UseEditedFields")]
        public bool UseEditedFields
        {
            get { return _useEditedFields; }
            set
            {
                _useEditedFields = value;
                OnPropertyChanged("UseEditedFields");
            }
        }

        #region ObjectStores

        [XmlElement(DataType = "string", ElementName = "SelectedSrcObjectStore")]
        public string SelectedSrcObjectStore { get; set; }

        [XmlElement(DataType = "string", ElementName = "SelectedSrcClass")]
        public string SelectedSrcClass { get; set; }

        [XmlElement(DataType = "string", ElementName = "SelectedDestObjectStore")]
        public string SelectedDestObjectStore { get; set; }

        [XmlElement(DataType = "string", ElementName = "SelectedDestClass")]
        public string SelectedDestClass { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
