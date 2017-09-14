using FileNetMigrationManager.Entities;
using System.ComponentModel;
using System.Windows;

namespace FileNetMigrationManager.Views
{
    /// <summary>
    /// Interaction logic for AuditViewer.xaml
    /// </summary>
    public partial class AuditViewer : Window, INotifyPropertyChanged
    {
        private AuditRecord _auditItem;

        public AuditRecord AuditItem
        {
            get { return _auditItem; }
            set
            {
                _auditItem = value;
                OnPropertyChanged("AuditItem");
            }
        }

        public AuditViewer(AuditRecord auditRecord)
        {
            InitializeComponent();
            this.DataContext = this;
            this.AuditItem = auditRecord;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintSystem ps = new PrintSystem();
            ps.PrintAuditRecord(_auditItem);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
