using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows;

namespace FileNetMigrationManager.Views
{
    /// <summary>
    /// Interaction logic for ProcessReport.xaml
    /// </summary>
    public partial class ProcessReport : Window
    {
        List<string> message = new List<string>();
        private Font printFont;

        public ProcessReport(int parentCount, int versionCount, int childCount, int processedBefore, int failCount, DateTime startTime, DateTime stopTime, bool chkExLog)
        {
            InitializeComponent();

            message.Add("Migration Processing Summary:");
            message.Add("    ");
            if (chkExLog)
            {
                message.Add("*The exception log contains new errors. Please review these errors to resolve.");
                message.Add("    ");
            }

            int totalProcessed = parentCount + versionCount + childCount + processedBefore + failCount;
            message.Add("Total Documents Processed: " + totalProcessed);
            int successCount = parentCount + versionCount + childCount;
            message.Add("Succeeded: " + successCount);
            message.Add("    Parent Documents: " + parentCount);
            message.Add("    Versioned Documents: " + versionCount);
            message.Add("    Child Documents: " + childCount);
            message.Add("Processed Before: " + processedBefore);
            message.Add("Failed: " + failCount);
            message.Add("    ");
            message.Add("Start Time: " + startTime.ToString());
            message.Add("Stop Time: " + stopTime.ToString());

            TimeSpan timeSpan = new TimeSpan(stopTime.Ticks - startTime.Ticks);
            message.Add("Total Processing Time (H:M:S): " + timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds);
            message.Add("    ");
            message.Add("    ");
            message.Add("(Parent Documents) Current version documents.");
            message.Add("(Versioned Documents) Previous versions of a current version document.");
            message.Add("(Child Documents) Child documents of a parent and all versions of that child document.");
            message.Add("(Processed Before) Documents that have already been successfully processed.");
            message.Add("(Failed) If a version or a child document fails, then the entire document is considered to have failed.");

            DisplaySummary();
        }

        private void DisplaySummary()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var line in message)
            {
                builder.AppendLine(line);
            }

            txtbSummary.Text = builder.ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintSystem ps = new PrintSystem(txtbSummary.Text.ToString());
            ps.Print();
        }
    }
}
