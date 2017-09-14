using FileNetMigrationManager.Entities;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace FileNetMigrationManager.ExportData
{
    public class Export
    {
        public static void ExportAuditList(List<AuditRecord> auditList)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                package.Workbook.Worksheets.Add("Audit List");
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Audit List"];

                worksheet.Column(1).Width = 45;
                worksheet.Column(2).Width = 45;
                worksheet.Column(3).Width = 45;
                worksheet.Column(4).Width = 45;
                worksheet.Column(5).Width = 25;
                worksheet.Column(6).Width = 10;
                worksheet.Column(7).Width = 50;
                worksheet.Column(8).Width = 50;

                worksheet.Cells[1, 1].Value = "Source Document ID";
                worksheet.Cells[1, 2].Value = "Source Version ID";
                worksheet.Cells[1, 3].Value = "Destination Document ID";
                worksheet.Cells[1, 4].Value = "Destination Version ID";
                worksheet.Cells[1, 5].Value = "Date";
                worksheet.Cells[1, 6].Value = "Success";
                worksheet.Cells[1, 7].Value = "Error Message";
                worksheet.Cells[1, 8].Value = "Stack Trace";

                int i = 2;
                foreach (var item in auditList)
                {
                    worksheet.Cells[i, 1].Value = item.SourceDocId;
                    worksheet.Cells[i, 2].Value = item.SourceDocVersionId;
                    worksheet.Cells[i, 3].Value = item.DestId;
                    worksheet.Cells[i, 4].Value = item.DestVersionId;
                    worksheet.Cells[i, 5].Value = item.DateProcessed.ToString();
                    worksheet.Cells[i, 6].Value = item.IsSuccess.ToString();
                    worksheet.Cells[i, 7].Value = item.EventMessage;
                    worksheet.Cells[i, 8].Value = item.ErrorString;

                    i++;
                }

                string filePath = Utilities.SaveFile("Select File Name", "Microsoft Excel|*.xlsx", 0, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
                if (!string.IsNullOrEmpty(filePath))
                {
                    FileInfo fi = new FileInfo(filePath);
                    package.SaveAs(fi);
                }
            }  
        }
    }
}
