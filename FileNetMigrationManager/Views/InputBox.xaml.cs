using System.ComponentModel;
using System.Windows;

namespace FileNetMigrationManager
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window, INotifyPropertyChanged
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public InputBox(string value = "", string title = "Enter Template Name")
        {
            InitializeComponent();
            this.DataContext = this;
            this.Value = value;
            this.Title = title;
            txtValue.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Save();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                if (_value.Length > 100)
                {
                    Utilities.ShowWarningMessageBox("A maximum length of 100 characters is allowed");
                    return;
                }

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                Utilities.ShowWarningMessageBox("A template name is required");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
