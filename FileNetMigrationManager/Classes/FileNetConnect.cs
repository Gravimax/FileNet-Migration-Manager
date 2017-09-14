using FileNet.Api.Authentication;
using FileNet.Api.Collection;
using FileNet.Api.Core;
using FileNet.Api.Exception;
using FileNet.Api.Query;
using FileNet.Api.Util;
using System;
using System.Collections.Generic;

namespace FileNetMigrationManager
{
    public class FileNetConnect : IDisposable
    {
        public string Url { private get; set; }

        public string Username { private get; set; }

        public string Password { private get; set; }

        public IDomain Domain { get; set; }

        public FileNetConnect()
        {

        }

        public FileNetConnect(string userName, string password, string url)
        {
            this.Url = url;
            this.Username = userName;
            this.Password = password;

            EstablishConnection(true);
        }

        /// <summary>
        /// Establishes the connection.
        /// </summary>
        /// <param name="getdomain">if set to <c>true</c> [getdomain].</param>
        /// <exception cref="Exception">Url and credentials must be set before connection can be established</exception>
        public void EstablishConnection(bool getdomain)
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                throw new Exception("Url and credentials must be set before connection can be established");
            }

            UsernameCredentials creds = new UsernameCredentials(Username, Password);
            ClientContext.SetProcessCredentials(creds);

            IConnection connection = Factory.Connection.GetConnection(Url);

            if (getdomain)
            {
                Domain = Factory.Domain.FetchInstance(connection, null, null);
            }
        }

        public bool ValidateConnection()
        {
            try
            {
                if (string.IsNullOrEmpty(Url)) { return false; }
                if (string.IsNullOrEmpty(Username)) { return false; }
                if (string.IsNullOrEmpty(Password)) { return false; }

                IConnection conn = Factory.Connection.GetConnection(Url);
                UsernameCredentials creds = new UsernameCredentials(Username, Password);
                ClientContext.SetProcessCredentials(creds);

                // Get default domain to verify connection.
                IDomain domain = Factory.Domain.FetchInstance(conn, null, null);

                if (domain != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                if (ex is EngineRuntimeException)
                {
                    EngineRuntimeException fnEx = (EngineRuntimeException)ex;
                    var exCode = fnEx.GetExceptionCode();

                    if (exCode.Equals(ExceptionCode.E_NOT_AUTHENTICATED) || exCode.Equals(ExceptionCode.E_ACCESS_DENIED) || exCode.Equals(ExceptionCode.TRANSPORT_WSI_NETWORK_ERROR))
                    {
                        return false;
                    }
                    else
                    {
                        throw ex;
                    }
                }

                throw ex;
            }
        }

        /// <summary>
        /// Gets the object stores.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Connection not established</exception>
        public IObjectStoreSet GetObjectStores()
        {
            if (Domain != null)
            {
                return Domain.ObjectStores;
            }

            throw new Exception("Connection not established");
        }

        /// <summary>
        /// Gets the document classes.
        /// </summary>
        /// <param name="objStore">The object store.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Connection not established</exception>
        public IClassDefinitionSet GetDocumentClasses(IObjectStore objStore)
        {
            if (objStore != null)
            {
                return objStore.RootClassDefinitions;
            }

            throw new Exception("Connection not established");
        }

        public IDocument FetchDocument(IObjectStore objStore, string docId)
        {
            return Factory.Document.FetchInstance(objStore, new FileNet.Api.Util.Id(docId), null);
        }

        /// <summary>
        /// Searches the documents using a sql string.
        /// </summary>
        /// <param name="objStore">The object store.</param>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public List<DocumentSearchItem> SearchDocuments(IObjectStore objStore, string sql)
        {
            EstablishConnection(false);

            SearchSQL query = new SearchSQL(sql);
            SearchScope searchScope = new SearchScope(objStore);

            List<DocumentSearchItem> results = new List<DocumentSearchItem>();

            // Use default max count
            IRepositoryRowSet ios = searchScope.FetchRows(query, null, null, true);
            IPageEnumerator pageEnum = ios.GetPageEnumerator();

            while (pageEnum.NextPage())
            {
                foreach (IRepositoryRow row in pageEnum.CurrentPage)
                {
                    results.Add(new DocumentSearchItem
                    {
                        Id = row.Properties.GetProperty("Id").GetIdValue().ToString()
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// Creates the search string.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="tzOffset">The tz offset.</param>
        /// <returns></returns>
        public string CreateSearchString(ProjectTemplate template, string className, int tzOffset)
        {
            string sqlPrt = "";

            if (template.FromDate != null && template.FromDate.HasValue)
            {
                string fromDte = template.FromDate.Value.ToString("yyyyMMdd") + FormatTimeZoneOffset(tzOffset); // The datetime format must be in this specific format. The time can be updated if necessary, but any other will fail. Do not change!!!
                sqlPrt += string.Format("and DateCreated >= {0} ", fromDte);
            }

            if (template.ToDate != null && template.ToDate.HasValue)
            {
                string toDte = template.ToDate.Value.AddDays(1).ToString("yyyyMMdd") + FormatTimeZoneOffset(tzOffset); // Adjusting for utc. FileNet stores datetimes as UTC.
                sqlPrt += string.Format("and DateCreated <= {0} ", toDte);
            }

            return string.Format("select Id, DateCreated from {0} where IsCurrentVersion = true {1}order by DateCreated ASC", className, sqlPrt);
        }

        /// <summary>
        /// Formats the timezone offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public string FormatTimeZoneOffset(int offset)
        {
            if (offset >= 0 && offset <= 9) // If single digit
            {
                return "T0" + offset + "0000Z";
            }
            else // Double digits
            {
                return "T" + offset + "0000Z";
            }
        }

        #region Dispose

        bool disposed = false;

        System.Runtime.InteropServices.SafeHandle handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();

                // Free any other managed objects here.
            }

            Domain = null;
            // Free any unmanaged objects here.
            disposed = true;
        }

        #endregion
    }
}
