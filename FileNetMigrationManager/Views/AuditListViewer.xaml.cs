using FileNetMigrationManager.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileNetMigrationManager.Views
{
    /// <summary>
    /// Interaction logic for AuditViewer.xaml
    /// </summary>
    public partial class AuditListViewer : Window, INotifyPropertyChanged
    {
        private List<AuditRecord> origionalList;

        private List<AuditRecord> _auditList;

        public List<AuditRecord> AuditList
        {
            get { return _auditList; }
            set
            {
                _auditList = value;
                OnPropertyChanged("AuditList");
            }
        }

        public AuditRecord SelectedRecord { get; set; }

        public DateTime? SelectedDate { get; set; }

        public DelegateCommand SortCommand { get; private set; }
        public DelegateCommand PrintCommand { get; private set; }
        public DelegateCommand ViewRecordCommand { get; private set; }


        public AuditListViewer(List<AuditRecord> auditList)
        {
            InitializeComponent();
            this.DataContext = this;

            this.AuditList = auditList;
            origionalList = auditList.ToList();

            SortCommand = new DelegateCommand(Sort);
            PrintCommand = new DelegateCommand(Print);
            ViewRecordCommand = new DelegateCommand(ViewRecord);
        }

        private void ViewRecord(object args)
        {
            AuditViewer viewer = new AuditViewer(SelectedRecord);
            viewer.Owner = this;
            viewer.ShowDialog();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedRecord != null)
            {
                AuditViewer viewer = new AuditViewer(SelectedRecord);
                viewer.Owner = this;
            }
        }

        private void Print(object args)
        {
            if (SelectedRecord != null)
            {
                PrintSystem ps = new PrintSystem();
                ps.PrintAuditRecord(SelectedRecord);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(PadString("Source Document ID", 65, ' ') + PadString("Destination Document ID", 60, ' ') + PadString("Date Processed", 40, ' ') + "Success");
            builder.AppendLine("");

            foreach (var item in _auditList)
            {
                builder.AppendLine(PadString(item.SourceDocId, 50) + PadString(item.DestId, 50) + PadString(item.DateProcessed.ToString(), 35) + item.IsSuccess);
            }

            PrintSystem ps = new PrintSystem(builder.ToString(), new System.Drawing.Printing.Margins(50, 50, 50, 50));
            ps.Print(true);
        }

        private string PadString(string data, int padLength, char spacer = ' ')
        {
            if (data.Length < padLength)
            {
                string temp = data.PadRight(padLength, spacer);
                return temp;
            }

            return data;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportData.Export.ExportAuditList(AuditList);
        }


        #region Filter


        private void txtGuid_KeyUp(object sender, KeyEventArgs e)
        {
            FilterByGuid(txtGuid.Text);
        }

        private void FilterByGuid(string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                AuditList = origionalList.Where(x => x.SourceDocId.Contains(guid) || x.SourceDocVersionId.Contains(guid) || x.DestId.Contains(guid) || x.DestVersionId.Contains(guid)).ToList();
            }
            else
            {
                AuditList = origionalList.ToList();
            }
        }

        private void dpProcessDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterByDate(SelectedDate);
        }

        private void FilterByDate(DateTime? dateTime)
        {
            if (dateTime != null && dateTime.HasValue)
            {
                AuditList = origionalList.Where(x => x.DateProcessed.Date == dateTime.Value.Date).ToList();
            }
            else
            {
                AuditList = origionalList.ToList();
            }
        }


        #endregion


        #region Sort

        ListSortDirection _sortDirection;
        string _sortColumn;

        private void Sort(object args)
        {
            string column = args as string;
            if (column != null)
            {
                if (_sortColumn == column)
                {
                    // Toggle sorting direction 
                    _sortDirection = _sortDirection == ListSortDirection.Ascending ?
                                                       ListSortDirection.Descending :
                                                       ListSortDirection.Ascending;
                }
                else
                {
                    _sortColumn = column;
                    _sortDirection = ListSortDirection.Ascending;
                }

                if (_sortDirection == ListSortDirection.Ascending)
                {
                    OrderByAscending(column);
                }
                else
                {
                    OrderByDecending(column);
                }
            }
        }

        private void OrderByAscending(string column)
        {
            switch (column)
            {
                case "Date":
                    AuditList = AuditList.OrderBy(x => x.DateProcessed).ToList();
                    break;
                case "Success":
                    AuditList = AuditList.OrderBy(x => x.IsSuccess).ToList();
                    break;
                default:
                    break;
            }
        }

        private void OrderByDecending(string column)
        {
            switch (column)
            {
                case "Date":
                    AuditList = AuditList.OrderByDescending(x => x.DateProcessed).ToList();
                    break;
                case "Success":
                    AuditList = AuditList.OrderByDescending(x => x.IsSuccess).ToList();
                    break;
                default:
                    break;
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
