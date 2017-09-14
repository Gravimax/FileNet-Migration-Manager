using FileNetMigrationManager.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileNetMigrationManager
{
    /// <summary>
    /// This class to handle the database functions
    /// </summary>
    public class DBBusiness
    {
        private string filePath = Utilities.GetCurrentDirectory() + "//AuditList.xml";
        private string auditArchive = Utilities.GetCurrentDirectory() + "//Audit Archive";
        public List<AuditRecord> AuditList = new List<AuditRecord>();

        #region Management

        public void LoadAuditTable()
        {
            if (Utilities.FileExists(filePath))
            {
                AuditList = Utilities.DeserializeFromFile<List<AuditRecord>>(filePath);
            }
        }

        public void SaveAuditTable()
        {
            Utilities.SerializeToFile<List<AuditRecord>>(filePath, AuditList);
        }

        public string ArchiveAuditTable()
        {
            if (!Utilities.FolderExists(auditArchive))
            {
                Utilities.CreateDirectory(auditArchive);
            }

            string currentDT = Utilities.DateTimeToFileFriendly(DateTime.Now.ToString());
            string fileName = auditArchive + "//AuditList " + currentDT + ".xml";

            Utilities.SerializeToFile<List<AuditRecord>>(fileName, AuditList);
            return "AuditList " + currentDT + ".xml";
        }

        public List<string> LoadAuditArchiveList()
        {
            List<string> temp = new List<string>();

            DirectoryInfo di = new DirectoryInfo(auditArchive);

            if (!di.Exists)
            {
                di.Create();
            }

            foreach (var item in di.GetFiles("*.xml"))
            {
                if (item.Name.StartsWith("AuditList"))
                {
                    temp.Add(item.Name);
                }
            }

            return temp;
        }

        public void LoadAuditArchive(string fileName)
        {
            AuditList = Utilities.DeserializeFromFile<List<AuditRecord>>(auditArchive + "//" + fileName);
        }

        public void CreateNewAuditList()
        {
            AuditList = new List<AuditRecord>();
        }

        #endregion

        #region Main

        /// <summary>
        /// Insert or update document into the audit table.
        /// </summary>
        /// <param name="SrcDocID">The source document id</param>
        /// <param name="SrcDocVSID">The source document version id</param>
        /// <param name="IsSuccess">Is the operation a success?</param>
        /// <param name="ErrorStr">Detailed error message</param>
        /// <param name="DestID">The destination document id</param>
        /// <param name="DestVSID">The destination document version id</param>
        /// <param name="excMessage">Error message</param>
        /// <returns></returns>
        public bool AuditDocument(string SrcDocID, string SrcDocVSID, bool IsSuccess, string ErrorStr, string DestID, string DestVSID, string excMessage)
        {
            if (!CheckDocumentAudit(SrcDocID))
            {
                return AddAuditDocument(SrcDocID, SrcDocVSID, IsSuccess, ErrorStr, DestID, DestVSID, excMessage);
            }
            else
            {
                return UpdateAuditDocument(SrcDocID, SrcDocVSID, IsSuccess, ErrorStr, DestID, DestVSID, excMessage);
            }
        }

        /// <summary>
        /// Insert document into the audit table.
        /// </summary>
        /// <param name="SrcDocID">The source document id</param>
        /// <param name="SrcDocVSID">The source document version id</param>
        /// <param name="IsSuccess">Is the operation a success?</param>
        /// <param name="ErrorStr">Detailed error message</param>
        /// <param name="DestID">The destination document id</param>
        /// <param name="DestVSID">The destination document version id</param>
        /// <param name="excMessage">Error message</param>
        /// <returns></returns>
        public bool AddAuditDocument(string SrcDocID, string SrcDocVSID, bool IsSuccess, string ErrorStr, string DestID, string DestVSID, string excMessage)
        {
            try
            {
                AuditList.Add(new AuditRecord
                {
                    DateProcessed = DateTime.Now,
                    DestId = DestID,
                    DestVersionId = DestVSID,
                    EventMessage = excMessage,
                    ErrorString = ErrorStr,
                    IsSuccess = IsSuccess,
                    SourceDocId = SrcDocID,
                    SourceDocVersionId = SrcDocVSID
                });

                return true;
            }
            catch (Exception ex)
            {
                Utilities.ShowMessageBox(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Update document into the audit table.
        /// </summary>
        /// <param name="SrcDocID">The source document id</param>
        /// <param name="IsSuccess">Is the operation a success?</param>
        /// <param name="ErrorStr">Detailed error message</param>
        /// <param name="DestID">The destination document id</param>
        /// <param name="DestVSID">The destination document version id</param>
        /// <param name="excMessage">Error message</param>
        /// <returns></returns>
        public bool UpdateAuditDocument(string SrcDocID, string SrcDocVSID, bool IsSuccess, string ErrorStr, string DestID, string DestVSID, string excMessage)
        {
            try
            {
                AuditRecord auditRecord = AuditList.FirstOrDefault(x => x.SourceDocId == SrcDocID);

                auditRecord.DateProcessed = DateTime.Now;
                auditRecord.DestId = DestID;
                auditRecord.DestVersionId = DestVSID;
                auditRecord.EventMessage = excMessage;
                auditRecord.ErrorString = ErrorStr;
                auditRecord.IsSuccess = IsSuccess;
                auditRecord.SourceDocId = SrcDocID;
                auditRecord.SourceDocVersionId = SrcDocVSID;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the document audit already exists.
        /// </summary>
        /// <param name="SrcDocID">The source document identifier.</param>
        /// <returns></returns>
        public bool CheckDocumentAudit(string SrcDocID)
        {
            try
            {
                return AuditList.FirstOrDefault(x => x.SourceDocId == SrcDocID) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the document is already processed
        /// </summary>
        /// <param name="SrcDocID"></param>
        /// <returns></returns>
        public bool CheckDocumentIsProcessed(string SrcDocID)
        {
            try
            {
                return AuditList.FirstOrDefault(x => x.SourceDocId == SrcDocID && x.IsSuccess == true) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
