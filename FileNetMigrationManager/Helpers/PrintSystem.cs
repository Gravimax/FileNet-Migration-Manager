using FileNetMigrationManager.Entities;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Controls;

namespace FileNetMigrationManager
{
    public class PrintSystem : IDisposable
    {
        private PrintDocument pd;
        public bool Landscape;
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private Font _font = new Font("Times New Roman", 10);

        public Font PrinterFont
        {
            get { return _font; }
            set { _font = value; }
        }

        private Margins _margin = new Margins();

        /// <summary>
        /// Defaults to 1 inch margins.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public Margins Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }


        #region Ctors


        public PrintSystem()
        {
            this.Text = string.Empty;
            pd = new PrintDocument();
            pd.PrintPage += OnPrintPage;
        }


        public PrintSystem(string text)
        {
            this._text = text;
            pd = new PrintDocument();
            pd.PrintPage += OnPrintPage;
        }

        public PrintSystem(string text, bool landscape)
        {
            this._text = text;
            this.Landscape = landscape;
            pd = new PrintDocument();
            pd.PrintPage += OnPrintPage;
        }


        public PrintSystem(string text, Margins margins)
        {
            this._text = text;
            this.Margin = margins;
            pd = new PrintDocument();
            pd.PrintPage += OnPrintPage;
        }


        public PrintSystem(string text, Margins margins, bool landscape)
        {
            this._text = text;
            this.Margin = margins;
            this.Landscape = landscape;
            pd = new PrintDocument();
            pd.PrintPage += OnPrintPage;
        }


        #endregion


        #region Main print methods


        public void Print()
        {
            StartPrint();
        }

        public void Print(bool landscape)
        {
            this.Landscape = landscape;
            StartPrint();
        }

        public void Print(Margins margins, bool landscape)
        {
            Margin = margins;
            Landscape = landscape;
            StartPrint();
        }

        public void Print(Font font, bool landscape)
        {
            PrinterFont = font;
            Landscape = landscape;
            StartPrint();
        }

        public void Print(Margins margins, Font font, bool landscape)
        {
            PrinterFont = font;
            Margin = margins;
            Landscape = landscape;
            StartPrint();
        }


        private void StartPrint()
        {
            if (!string.IsNullOrEmpty(_text))
            {
                pd.DefaultPageSettings.Landscape = this.Landscape;
                pd.DefaultPageSettings.Margins = Margin;

                if (_font == null)
                {
                    _font = new Font("Times New Roman", 10);
                }

                PrintDialog printDialog = new PrintDialog();
                
                if (printDialog.ShowDialog() == true)
                {
                    pd.PrinterSettings.PrinterName = printDialog.PrintQueue.Name; // PrintQueue.Name is the selected printer name
                    pd.Print();
                }
            }
        }


        #endregion


        public void PrintAuditRecord(AuditRecord auditRecord)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Source Document ID: " + auditRecord.SourceDocId);
            builder.AppendLine(" ");
            builder.AppendLine("Source Version ID: " + auditRecord.SourceDocVersionId);
            builder.AppendLine(" ");
            builder.AppendLine("Destination Doc ID: " + auditRecord.DestId);
            builder.AppendLine(" ");
            builder.AppendLine("Destination Version ID: " + auditRecord.DestVersionId);
            builder.AppendLine(" ");
            builder.AppendLine("Date Processed " + auditRecord.DateProcessed.ToString());
            builder.AppendLine(" ");
            builder.AppendLine("Success: " + auditRecord.IsSuccess);
            builder.AppendLine(" ");
            builder.AppendLine("Event Message: " + auditRecord.EventMessage);
            builder.AppendLine(" ");
            builder.AppendLine("Stack Trace:");
            builder.AppendLine(" ");
            builder.AppendLine(auditRecord.ErrorString);

            this.Text = builder.ToString();
            _font = new Font("Times New Roman", 12);
            Margin = new Margins();
            this.Landscape = false;

            StartPrint();
        }


        private void OnPrintPage(object sender, PrintPageEventArgs e)
        {            
            //int printHeight;
            //int printWidth;
            //int leftMargin;
            //int rightMargin;
            int linesPerPage;
            int charactersOnPage;

            // Set print area and margins
            //printHeight = pd.DefaultPageSettings.PaperSize.Height - pd.DefaultPageSettings.Margins.Top - pd.DefaultPageSettings.Margins.Bottom;
            //printWidth = pd.DefaultPageSettings.PaperSize.Width - pd.DefaultPageSettings.Margins.Left - pd.DefaultPageSettings.Margins.Right;
            //leftMargin = pd.DefaultPageSettings.Margins.Left; // X
            //rightMargin = pd.DefaultPageSettings.Margins.Top; // Y

            //if (pd.DefaultPageSettings.Landscape)
            //{
            //    int tmp;
            //    tmp = printHeight;
            //    printHeight = printWidth;
            //    printWidth = tmp;
            //}

            // Create a rectangle print area for our doc
            //RectangleF printArea = new RectangleF(leftMargin, rightMargin, printWidth, printHeight);

            // Use the StringFormat class for the text layout of the doc
            //StringFormat format = new StringFormat(StringFormatFlags.LineLimit);

            // Fit as many chars as possible into print area. .Substring(RemovesZeros(curChar))
            e.Graphics.MeasureString(_text, PrinterFont, e.MarginBounds.Size, StringFormat.GenericTypographic, out charactersOnPage, out linesPerPage);

            // Print the page
            e.Graphics.DrawString(_text, PrinterFont, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic);

            //curChar += charactersOnPage;

            Text = Text.Substring(charactersOnPage);

            if (Text.Length > 0)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }
        }


        /// <summary>
        /// Zero's can cause bad things to happen when it comes to printing and determining page size and margins.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int RemovesZeros(int value)
        {
            return value == 0 ? 1 : value;
        }


        #region Dispose

        
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);


        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            pd.Dispose();
            pd = null;
            // Free any unmanaged objects here.
            //
            disposed = true;
        }


        #endregion
    }
}
