﻿using System;
using System.Diagnostics;
using System.Reflection;

namespace FileNetMigrationManager
{
    /// <summary>
    /// http://www.bunkerhollow.com/blogs/matt/archive/2009/04/29/net-get-assembly-information-title-version-etc.aspx
    /// </summary>
    public static class AssemblyInfoHelper
    {
        /// <summary>
        /// Version of the current assembly.
        /// </summary>
        /// <param name="includeMinor">if set to <c>true</c> [include minor].</param>
        /// <param name="includeBuild">if set to <c>true</c> [include build].</param>
        /// <param name="includeRevision">if set to <c>true</c> [include revision].</param>
        /// <returns>Assembly version as a string in Major.Minor.Build.Revision format.</returns>
        public static string AssemblyVersionDetails(bool includeMinor, bool includeBuild, bool includeRevision)
        {
            string pVersion = string.Empty;

            try
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                pVersion = version.Major.ToString();

                if (includeMinor) { pVersion += "." + version.Minor.ToString(); }

                if (includeBuild)
                {
                    if (!includeMinor) { pVersion += ".x"; }
                    pVersion += "." + version.Build.ToString();
                }

                if (includeRevision)
                {
                    if (!includeBuild) { pVersion += ".x"; }
                    pVersion += "." + version.Revision.ToString();
                }
            }
            catch
            {
                pVersion = "0.0.0.0";
            }

            return pVersion;
        }

        public static string Title
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyTitleAttribute)customAttributes[0]).Title;
                }

                return result;
            }
        }

        public static string Description
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
                }

                return result;
            }
        }

        public static string Company
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyCompanyAttribute)customAttributes[0]).Company;
                }

                return result;
            }
        }

        public static string Product
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyProductAttribute)customAttributes[0]).Product;
                }
                return result;
            }
        }

        public static string Copyright
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
                }
                return result;
            }
        }

        public static string Trademark
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyTrademarkAttribute)customAttributes[0]).Trademark;
                }
                return result;
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
        }

        public static string FileVersion
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public static string Guid
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((System.Runtime.InteropServices.GuidAttribute)customAttributes[0]).Value;
                }
                return result;
            }
        }

        public static string FileName
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.OriginalFilename;
            }
        }

        public static string FilePath
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileName;
            }
        }
    }
}