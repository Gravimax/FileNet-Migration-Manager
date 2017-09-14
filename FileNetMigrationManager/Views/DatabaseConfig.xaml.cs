using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace FileNetMigrationManager
{
    /// <summary>
    /// Interaction logic for DatabaseConfig.xaml
    /// </summary>
    public partial class DatabaseConfig : Window, INotifyPropertyChanged
    {
        DBBusiness recordAudit;

        private List<string> _archiveList;

        public List<string> ArchiveList
        {
            get { return _archiveList; }
            set
            {
                _archiveList = value;
                OnPropertyChanged("ArchiveList");
            }
        }

        public string SelectedArchive { get; set; }

        public DatabaseConfig(DBBusiness recordAudit)
        {
            InitializeComponent();

            this.DataContext = this;
            this.recordAudit = recordAudit;
            GetDBArchives();
        }

        private void GetDBArchives()
        {
            ArchiveList = recordAudit.LoadAuditArchiveList();
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedArchive))
            {
                MessageBoxResult result = MessageBox.Show("This will load the selected audit database./r/nDo you want the current audit database archived?", "Archive Current Audit Database", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

                if (result == MessageBoxResult.Yes)
                {
                    recordAudit.ArchiveAuditTable();
                }

                recordAudit.LoadAuditArchive(SelectedArchive);
                MessageBox.Show("Adit database loaded", "Database Loaded", MessageBoxButton.OK);
            }
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            recordAudit.ArchiveAuditTable();
            recordAudit.CreateNewAuditList();
            MessageBox.Show("New audit database created", "New Database", MessageBoxButton.OK);
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            ArchiveList.Add(recordAudit.ArchiveAuditTable());
            MessageBox.Show("Audit database backed up", "Database Backedup", MessageBoxButton.OK);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            recordAudit.CreateNewAuditList();
            MessageBox.Show("Audit database cleared", "Database Cleared", MessageBoxButton.OK);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
