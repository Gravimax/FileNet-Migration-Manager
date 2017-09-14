using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FileNetMigrationManager
{
    [Serializable]
    public class DatabaseConfiguration : INotifyPropertyChanged
    {
        private int _authenticationType;
        [XmlElement(DataType = "int", ElementName = "AuthenticationType")]
        public int AuthenticationType
        {
            get { return _authenticationType; }
            set
            {
                _authenticationType = value;
                OnPropertyChanged("AuthenticationType");
            }
        }

        private string _sqlServerName;
        [XmlElement(DataType = "string", ElementName = "SqlServerName")]
        public string SqlServerName
        {
            get { return _sqlServerName; }
            set
            {
                _sqlServerName = value;
                OnPropertyChanged("SqlServerName");
            }
        }

        private string _databaseName;
        [XmlElement(DataType = "string", ElementName = "DatabaseName")]
        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                _databaseName = value;
                OnPropertyChanged("DatabaseName");
            }
        }

        private string _userName;
        [XmlElement(DataType = "string", ElementName = "UserName")]
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string _secPassword;
        [XmlElement(DataType = "string", ElementName = "SecPassword")]
        public string SecPassword
        {
            get { return _secPassword; }
            set { _secPassword = value; }
        }

        [XmlIgnore]
        public string Password
        {
            get { return EncryptionUtilities.StringCipher.Decrypt(_secPassword); }
            set
            {
                _secPassword = EncryptionUtilities.StringCipher.Encrypt(value);
                OnPropertyChanged("Password");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
