using System;
using System.Security.Principal;
using System.Threading;
using System.Windows;

namespace FileNetMigrationManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ApplicationViewModel vm = new ApplicationViewModel();

            vm.ExitApplication += this.OnExitApplication;
            vm.SetSrcPassword += this.OnSetSrcPassword;
            vm.SetDestPassword += this.OnSetDestPassword;

            this.DataContext = vm;
        }

        // Sets the password in the source passwordbox after template loads
        private void OnSetSrcPassword(object sender, PasswordEventArgs e)
        {
            srcServer.pswdSrcPassword.Password = e.Password;
        }

        // Sets the password in the destination passwordbox after template loads
        private void OnSetDestPassword(object sender, PasswordEventArgs e)
        {
            destServer.pswdDestPassword.Password = e.Password;
        }

        private void OnExitApplication(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ApplicationViewModel vm = this.DataContext as ApplicationViewModel;
            //vm.LoadMostRecentTemplate(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplicationViewModel vm = this.DataContext as ApplicationViewModel;
            if (vm != null)
            {
                vm.SaveMtomUrls();

                if (Utilities.ShowDialogBox("Save current template?") == true)
                {
                    vm.SaveTemplate(this);
                }
            }
        }
    }
}
