using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace FileNetMigrationManager
{
    public static class Utilities
    {
        public static void ShowMessageBox(string message)
        {
            MessageBox.Show(message, AssemblyInfoHelper.Product, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message, AssemblyInfoHelper.Product, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowExceptionMessageBox(Exception ex)
        {
            MessageBox.Show(string.Format("Error:\n{0}", ex.Message.ToString()), AssemblyInfoHelper.Product, MessageBoxButton.OK, MessageBoxImage.Hand);
            WriteToExceptionLog(ex, null);
        }

        public static void ShowWarningMessageBox(string message)
        {
            MessageBox.Show(message, AssemblyInfoHelper.Product, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool ShowDialogBox(string message)
        {
            var result = MessageBox.Show(message, AssemblyInfoHelper.Product, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes || result == MessageBoxResult.OK)
            {
                return true;
            }

            return false;
        }

        private static object locker = new object();

        /// <summary>
        /// Writes to the log file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteToExceptionLog(Exception ex, string message)
        {
            lock (locker)
            {
                string errMessage = "";
                if (!string.IsNullOrEmpty(message)) errMessage = "[" + DateTime.Now + "] " + message + "\r\n";
                errMessage += "[" + DateTime.Now + "] Exception: " + ex.Message + "\r\n" + ex.StackTrace + "\r\n";

                if (ex.InnerException != null)
                {
                    errMessage += "[" + DateTime.Now + "] Inner Exception: " + ex.InnerException.Message + "\r\n" + ex.InnerException.StackTrace + "\r\n";
                }

                string fileName = Utilities.GetCurrentDirectory() + "\\ExceptionLog.log";
                File.AppendAllText(fileName, errMessage);
            }
        }

        public static string ReadTextFile(string path)
        {
            string temp = File.ReadAllText(path);
            return temp;
        }

        public static void WriteTextFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        /// <summary>
        /// Converts a datetime to a file friendly format.
        /// </summary>
        /// <param name="dateTime">The datetime as a string.</param>
        /// <returns>File friendly datetime string.</returns>
        public static string DateTimeToFileFriendly(string dateTime)
        {
            // Non-valid file name characters: \ / : * ? " < > |
            StringBuilder newDateTime = new StringBuilder(dateTime);
            newDateTime.Replace(',', ' ');
            newDateTime.Replace(':', '.');
            newDateTime.Replace('/', '-');
            newDateTime.Replace('<', '(');
            newDateTime.Replace('>', ')');

            return newDateTime.ToString();
        }

        public static string GetTemplatePath()
        {
            return System.IO.Path.Combine(GetCurrentDirectory(), "Templates");
        }

        public static string GetTemplateFilePath(string fileName)
        {
            return System.IO.Path.Combine(GetTemplatePath(), fileName + ".tmpl");
        }

        public static string GetCurrentDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static bool FileExists(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                FileInfo fi = new FileInfo(filePath);
                return fi.Exists;
            }
            else
            {
                throw new ArgumentNullException(nameof(filePath));
            }
        }

        public static bool FolderExists(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return Directory.Exists(path);
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        public static void CreateDirectory(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                throw new ArgumentNullException(nameof(path));
            }
        }

        /// <summary>
        /// Gets the file name without the extention.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>String</returns>
        public static string GetFileNameNoExtention(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                return Path.GetFileNameWithoutExtension(fileName);
            }
            return string.Empty;
        }

        public static List<string> GetTemplateList()
        {
            if (!System.IO.Directory.Exists(GetTemplatePath()))
            {
                System.IO.Directory.CreateDirectory(GetTemplatePath());
            }

            if (Directory.Exists(Path.Combine(GetCurrentDirectory(), "Templates")))
            {
                var templateList = Directory.GetFiles(GetTemplatePath(), "*.tmpl").ToList();

                return templateList.Select(x => GetFileNameNoExtention(x)).ToList();
            }

            return new List<string>();
        }

        /// <summary>
        /// Depricated.
        /// </summary>
        public static ProjectTemplate GetMostRecentTemplate()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(GetCurrentDirectory(), "Templates")))
                {
                    Directory.CreateDirectory(Path.Combine(GetCurrentDirectory(), "Templates"));
                }

                var directory = new DirectoryInfo(Path.Combine(GetCurrentDirectory(), "Templates"));
                var file = (from f in directory.GetFiles("*.tmpl")
                            orderby f.LastAccessTime descending
                            select f).FirstOrDefault();

                if (file != null)
                {
                    return DeserializeFromFile<ProjectTemplate>(file.FullName);
                }

                return new ProjectTemplate
                {
                    FromDate = DateTime.Now,
                    IsAllVersions = 0,
                    IsMove = 0,
                    ToDate = DateTime.Now,
                    PropertiesMapping = new System.Collections.ObjectModel.ObservableCollection<PropertiesMap>()
                };
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox(string.Format("Error:\n{0}", ex.Message.ToString()));

                return new ProjectTemplate
                {
                    FromDate = DateTime.Now,
                    IsAllVersions = 0,
                    IsMove = 0,
                    ToDate = DateTime.Now,
                    PropertiesMapping = new System.Collections.ObjectModel.ObservableCollection<PropertiesMap>()
                };
            }
        }

        public static DatabaseConfiguration LoadDatabaseConfig()
        {
            if (File.Exists(Path.Combine(Utilities.GetCurrentDirectory(), "DatabaseConfig.cfg")))
            {
                return DeserializeFromFile<DatabaseConfiguration>(Path.Combine(Utilities.GetCurrentDirectory(), "DatabaseConfig.cfg"));
            }

            return new DatabaseConfiguration { AuthenticationType = 1 };
        }

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="dfConfig">The df configuration.</param>
        /// <returns></returns>
        public static string GetDBConnectionString(DatabaseConfiguration dfConfig)
        {
            var secString = string.Empty;
            switch (dfConfig.AuthenticationType)
            {
                case 1:
                    secString = string.Format("User Id={0};Password={1};", dfConfig.UserName, dfConfig.Password);
                    break;
                case 2:
                    secString = "Integrated Security=SSPI;";
                    break;
                case 3:
                    secString = "Trusted_Connection=True;";
                    break;
                default:
                    break;
            }

            return string.Format(@"Provider=SQLOLEDB;Data Source={0};Initial Catalog={1};{2}", dfConfig.SqlServerName, dfConfig.DatabaseName, secString); //Provider=SQLOLEDB
        }

        /// <summary>
        /// Depricated.
        /// </summary>
        public static void ResetConnectionString()
        {
            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var connectionStrings = (ConnectionStringsSection)config.GetSection("connectionStrings");

            //connectionStrings.ConnectionStrings["MainConnString"].ConnectionString = @"Provider=sqloledb;Data Source=;Initial Catalog=;Persist Security Info=False;";

            //config.Save(ConfigurationSaveMode.Modified, true);
        }

        /// <summary>
        /// Selects a file.
        /// </summary>
        /// <param name="Title">The title.</param>
        /// <param name="Filter">The filter.</param>
        /// <param name="FilterIndex">Desired initial index of the filter.</param>
        /// <param name="InitialDir">The initial directory.</param>
        /// <returns>Path to the selected file or null.</returns>
        public static string SaveFile(string Title = "Select a file", string Filter = "All files|*.*", int FilterIndex = 1, string InitialDir = "C:\\")
        {
            SaveFileDialog fldg = new SaveFileDialog();
            fldg.Title = Title;
            fldg.Filter = Filter;
            fldg.FilterIndex = FilterIndex;
            fldg.InitialDirectory = InitialDir;
            fldg.RestoreDirectory = true;

            fldg.ShowDialog();

            return fldg.FileName;
        }

        public static List<string> LoadUrlList()
        {
            if (File.Exists(Path.Combine(Utilities.GetCurrentDirectory(), "MtomUrlList.cfg")))
            {
                UrlList list = DeserializeFromFile<UrlList>(Path.Combine(Utilities.GetCurrentDirectory(), "MtomUrlList.cfg"));
                return list.MtomUrls;
            }

            return new List<string>();
        }

        public static void SaveUrlList(List<string> urls)
        {
            UrlList list = new UrlList();
            list.MtomUrls = urls;
            SerializeToFile<UrlList>(Path.Combine(Utilities.GetCurrentDirectory(), "MtomUrlList.cfg"), list);
        }

        // Serialize to file
        /// <summary>
        /// Takes a serializable object and saves it to an xml file.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="path">The path including the file name.</param>
        /// <param name="obj">The object to serialize.</param>
        public static void SerializeToFile<T>(string path, T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter textWriter = new StreamWriter(path))
            {
                serializer.Serialize(textWriter, obj);
                textWriter.Close();
            }
        }

        /// <summary>
        /// Deserializes an object from a file.
        /// </summary>
        /// <param name="path">The path including the file name.</param>
        /// <returns>Object of type T</returns>
        public static T DeserializeFromFile<T>(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                XmlReader xmlReader = new XmlTextReader(stream);
                return (T)xmlSerializer.Deserialize(xmlReader);
            }
        }
    }
}
