using System.Windows;
using System.Windows.Input;

namespace FileNetMigrationManager
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            lblTitle.Content = AssemblyInfoHelper.Product;
            lblVersion.Content = "Verson: " + AssemblyInfoHelper.AssemblyVersionDetails(true, true, true);
            lblCopyright.Content = AssemblyInfoHelper.Copyright;
            string activationKey;
            Registration.ProductKeyManager pkm = new Registration.ProductKeyManager();
            pkm.CheckKey(out activationKey);
            if (!string.IsNullOrEmpty(activationKey))
            {
                lblActiveKey.Content = "Activation Key: " + activationKey;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lblUrlLink_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://uzer.io");
        }
    }
}
