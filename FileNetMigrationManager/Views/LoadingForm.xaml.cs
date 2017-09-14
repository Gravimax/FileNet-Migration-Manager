using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileNetMigrationManager
{
    /// <summary>
    /// Interaction logic for LoadingForm.xaml
    /// </summary>
    public partial class LoadingForm : Window, INotifyPropertyChanged, IDisposable
    {
        private string _status;

        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private double _progress;

        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");                
            }
        }

        System.Timers.Timer timer = null;

        public LoadingForm(string status = "", string message = "")
        {
            InitializeComponent();
            this.DataContext = this;
            this.Status = status;
            this.Message = message;

            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 100;
            timer.Enabled = true;
            timer.Start();
        }

        public void OnSetMessage(object sender, SetMessageEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Status = e.Message;
            }));
        }

        public void SetMessage(string message)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Status = message;
            }));
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (_progress >= 100)
                {
                    Progress = 0;
                    return;
                }
                else
                {
                    Progress += 10;
                }
            }));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Utilities.ShowDialogBox(string.Format("Are you sure, Do you want to cancel the process?")) == true)
                {
                    timer.Enabled = false;
                    timer = null;
                    this.Close();
                }
            }
            catch (Exception Ex)
            {
                Utilities.ShowMessageBox(string.Format("Error:\n{0}", Ex.Message.ToString()));
                //Utilities.ShowMessageBox(string.Format("Error:\n{0}\n\n---------------------------\n\nError details:\n{1}", Ex.Message.ToString(), Ex.ToString()));
            }
        }

        public event CancelOperationEventHandler CancelOperation;

        private void OnCancelOperation()
        {
            if (CancelOperation != null) { CancelOperation(this, EventArgs.Empty); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }
    }
}
