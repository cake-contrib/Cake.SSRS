using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace Cake.SSRS
{
    /// <summary>
    /// Contains functionality for working with SQL Server Reporting Services (SSRS)
    /// </summary>
    [CakeAliasCategory("SSRS")]
    [CakeNamespaceImport("Cake.SSRS")]
    public static class SsrsAliases
    {
        private static readonly List<string> ReadOnlyProperties = new List<string>() { "Name" };

        #region SsrsCreateFolder

        /// <summary>
        /// Creates a new Folder in SSRS if it does not exist
        /// </summary>
        /// <example>
        /// <code>
        ///    var catalogItem = SsrsCreateFolder("AdventureWorks", "/", new SsrsConnectionSettings
        ///    {
        ///        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        ///        UseDefaultCredentials = true
        ///    });
        /// </code>        
        /// </example>
        /// <param name="context">Cake Context</param>
        /// <param name="folderName">Name of the folder to create</param>
        /// <param name="parentFolder">Parent folder (if any) to create the new folder under</param>
        /// <param name="settings">SSRS Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Folders")]
        public static CatalogItem SsrsCreateFolder(this ICakeContext context, string folderName, string parentFolder, SsrsConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentNullException(nameof(folderName));

            if (string.IsNullOrWhiteSpace(parentFolder))
                parentFolder = "/";

            var client = GetReportingService(context, settings);
            
            var request = new FindItemRequest
            {
                Folder = parentFolder,
                ItemName = folderName,
                Recursive = false
            };

            var item = FindItem(context, client, settings, request);

            if (item != null)
            {
                context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Warning, "Folder {0}/{1} already exists on the SSRS server.", parentFolder, folderName);
                return item;
            }

            context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Information, "Created new SSRS folder: {0}/{1}.", parentFolder, folderName);

            var createFolderRequest = new CreateFolderRequest()
            {
                Folder = folderName,
                Parent = parentFolder
            };

            return client.CreateFolderAsync(createFolderRequest).GetAwaiter().GetResult()?.ItemInfo;
        }

        /// <summary>
        /// Creates a new Folder in SSRS if it does not exist
        /// </summary>
        /// <example>
        /// <code>
        ///     var catalogItem = SsrsCreateFolder("AdventureWorks", "/", configurator =>
        ///     {
        ///         configurator.ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx";
        ///         configurator.UseDefaultCredentials = true;
        ///     });
        /// </code>        
        /// </example>
        /// <param name="context">Cake Context</param>
        /// <param name="folderName">Name of the folder to create</param>
        /// <param name="parentFolder">Parent folder (if any) to create the new folder under</param>
        /// <param name="settingsConfigurator">SSRS Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns> 
        [CakeMethodAlias]
        [CakeAliasCategory("Folders")]
        public static CatalogItem SsrsCreateFolder(this ICakeContext context, string folderName, string parentFolder, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsCreateFolder(context, folderName, parentFolder, settings);
        }

        /// <summary>
        /// Remove a Folder in SSRS with all its content
        /// </summary>
        /// <example>
        /// <code>
        ///    var catalogItem = SsrsRemoveFolder("AdventureWorks", "/", new SsrsConnectionSettings
        ///    {
        ///        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        ///        UseDefaultCredentials = true
        ///    });
        /// </code>        
        /// </example>
        /// <param name="context">Cake Context</param>
        /// <param name="folderName">Name of the folder to remove</param>
        /// <param name="parentFolder">Parent folder (if any) to remove the existing folder under</param>
        /// <param name="settings">SSRS Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Folders")]
        public static void SsrsRemoveFolder(this ICakeContext context, string folderName, string parentFolder, SsrsConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentNullException(nameof(folderName));

            if (string.IsNullOrWhiteSpace(parentFolder))
                parentFolder = "/";

            var client = GetReportingService(context, settings);

            var request = new FindItemRequest
            {
                Folder = parentFolder,
                ItemName = folderName,
                Recursive = false
            };

            var item = FindItem(context, client, settings, request);

            if (item == null)
            {
                context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Warning, "Folder {0}/{1} did not exists on the SSRS server.", parentFolder, folderName);
            }
            else
            {
                var deleteFolderRequest = new DeleteItemRequest()
                {
                    ItemPath = item.Path
                };

                client.DeleteItemAsync(deleteFolderRequest).Wait();

                context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Information, "Removed existing SSRS folder: {0}/{1}.", parentFolder, folderName);
            }
        }

        /// <summary>
        /// Remove a Folder in SSRS with all its content
        /// </summary>
        /// <example>
        /// <code>
        ///     var catalogItem = SsrsRemoveFolder("AdventureWorks", "/", configurator =>
        ///     {
        ///         configurator.ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx";
        ///         configurator.UseDefaultCredentials = true;
        ///     });
        /// </code>        
        /// </example>
        /// <param name="context">Cake Context</param>
        /// <param name="folderName">Name of the folder to remove</param>
        /// <param name="parentFolder">Parent folder (if any) to remove the existing folder under</param>
        /// <param name="settingsConfigurator">SSRS Settings</param>
        [CakeMethodAlias]
        [CakeAliasCategory("Folders")]
        public static void SsrsRemoveFolder(this ICakeContext context, string folderName, string parentFolder, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            SsrsRemoveFolder(context, folderName, parentFolder, settings);
        }

        #endregion

        #region SsrsUploadReport

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified
        /// </summary>
        /// <example>
        /// <code>
        ///    var catalogItem = SsrsUploadReport("./path/to/report.rdl", "/AdventureWorks",
        ///            new Dictionary&lt;string, string&gt;
        ///            {
        ///                 ["Description"] = "Description for the Report"
        ///            },
        ///            new SsrsConnectionSettings
        ///            {
        ///                ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        ///                UseDefaultCredentials = true
        ///            });
        /// </code>        
        /// </example>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static CatalogItem SsrsUploadReport(this ICakeContext context, FilePath filePath, string folderPath, SsrsConnectionSettings settings)
        {
            return SsrsUploadReport(context, filePath, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static CatalogItem SsrsUploadReport(this ICakeContext context, FilePath filePath, string folderPath, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            return SsrsUploadReport(context, filePath, folderPath, new Dictionary<string, string>(), settingsConfigurator);
        }

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static CatalogItem SsrsUploadReport(this ICakeContext context, FilePath filePath, string folderPath, IDictionary<string, string> properties, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadReport(context, filePath, folderPath, properties, settings);
        }

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static CatalogItem SsrsUploadReport(this ICakeContext context, FilePath filePath, string folderPath, IDictionary<string, string> properties, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentNullException(nameof(folderPath));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return UploadCatalogItem(context, GetReportingService(context, settings), ItemType.Report, filePath, folderPath, properties);
        }

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static IEnumerable<CatalogItem> SsrsUploadReport(this ICakeContext context, string pattern, string folderPath, SsrsConnectionSettings settings)
        {
            return SsrsUploadReport(context, pattern, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static IEnumerable<CatalogItem> SsrsUploadReport(this ICakeContext context, string pattern, string folderPath, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadReport(context, pattern, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static IEnumerable<CatalogItem> SsrsUploadReport(this ICakeContext context, string pattern, string folderPath, IDictionary<string, string> properties, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadReport(context, pattern, folderPath, properties, settings);
        }
        
        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rdl file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("Reports")]
        public static IEnumerable<CatalogItem> SsrsUploadReport(this ICakeContext context, string pattern, string folderPath, IDictionary<string, string> properties, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentNullException(nameof(folderPath));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var items = new List<CatalogItem>();

            var filePaths = context.Globber.GetFiles(pattern);
            if(filePaths == null || filePaths.Count() < 1)
            {
                context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Warning, "No Report files found matching the pattern '{0}'", pattern);
                return items;
            }

            var client = GetReportingService(context, settings);

            foreach (var filePath in filePaths)
                items.Add(UploadCatalogItem(context, client, ItemType.Report, filePath, folderPath, properties));

            return items;
        }

        #endregion

        #region SsrsUploadDataSet

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static CatalogItem SsrsUploadDataSet(this ICakeContext context, FilePath filePath, string folderPath, SsrsConnectionSettings settings)
        {
            return SsrsUploadDataSet(context, filePath, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static CatalogItem SsrsUploadDataSet(this ICakeContext context, FilePath filePath, string folderPath, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            return SsrsUploadDataSet(context, filePath, folderPath, new Dictionary<string, string>(), settingsConfigurator);
        }

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static CatalogItem SsrsUploadDataSet(this ICakeContext context, FilePath filePath, string folderPath, IDictionary<string, string> properties, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadDataSet(context, filePath, folderPath, properties, settings);
        }

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified
        /// </summary>
        /// <example>
        /// <code>
        ///    var catalogItem = SsrsUploadReport("./path/to/dataset.rsd", "/AdventureWorks",
        ///            new Dictionary&lt;string, string&gt;
        ///            {
        ///                 ["Description"] = "Description for the DataSet"
        ///            },
        ///            new SsrsConnectionSettings
        ///            {
        ///                ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        ///                UseDefaultCredentials = true
        ///            });
        /// </code>        
        /// </example>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static CatalogItem SsrsUploadDataSet(this ICakeContext context, FilePath filePath, string folderPath, IDictionary<string, string> properties, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentNullException(nameof(folderPath));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return UploadCatalogItem(context, GetReportingService(context, settings), ItemType.DataSet, filePath, folderPath, properties);
        }

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSet(this ICakeContext context, string pattern, string folderPath, SsrsConnectionSettings settings)
        {
            return SsrsUploadDataSet(context, pattern, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSet(this ICakeContext context, string pattern, string folderPath, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadDataSet(context, pattern, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSet(this ICakeContext context, string pattern, string folderPath, IDictionary<string, string> properties, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadDataSet(context, pattern, folderPath, properties, settings);
        }

        /// <summary>
        /// Uploads an SSRS Shared DataSet (.rsd) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSets")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSet(this ICakeContext context, string pattern, string folderPath, IDictionary<string, string> properties, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentNullException(nameof(folderPath));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var items = new List<CatalogItem>();

            var filePaths = context.Globber.GetFiles(pattern);
            if (filePaths == null || filePaths.Count() < 1)
            {
                context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Warning, "No Shared DataSet files found matching the pattern '{0}'", pattern);
                return items;
            }

            var client = GetReportingService(context, settings);

            foreach (var filePath in filePaths)
                items.Add(UploadCatalogItem(context, client, ItemType.DataSet, filePath, folderPath, properties));

            return items;
        }

        #endregion

        #region SsrsUploadDataSource

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static CatalogItem SsrsUploadDataSource(this ICakeContext context, FilePath filePath, string folderPath, SsrsConnectionSettings settings)
        {
            return SsrsUploadDataSource(context, filePath, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static CatalogItem SsrsUploadDataSource(this ICakeContext context, FilePath filePath, string folderPath, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            return SsrsUploadDataSource(context, filePath, folderPath, new Dictionary<string, string>(), settingsConfigurator);
        }

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static CatalogItem SsrsUploadDataSource(this ICakeContext context, FilePath filePath, string folderPath, IDictionary<string, string> properties, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadDataSource(context, filePath, folderPath, properties, settings);
        }

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified
        /// </summary>
        /// <example>
        /// <code>
        ///    var catalogItem = SsrsUploadDataSource("./path/to/datasource.rds", "/AdventureWorks",
        ///            new Dictionary&lt;string, string&gt;
        ///            {
        ///                 ["Description"] = "Description for the DataSource"
        ///            },
        ///            new SsrsConnectionSettings
        ///            {
        ///                ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        ///                UseDefaultCredentials = true
        ///            });
        /// </code>        
        /// </example> 
        /// <param name="context">Cake Contex</param>
        /// <param name="filePath">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static CatalogItem SsrsUploadDataSource(this ICakeContext context, FilePath filePath, string folderPath, IDictionary<string, string> properties, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentNullException(nameof(folderPath));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return UploadCatalogItem(context, GetReportingService(context, settings), ItemType.DataSource, filePath, folderPath, properties);
        }

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSource(this ICakeContext context, string pattern, string folderPath, SsrsConnectionSettings settings)
        {
            return SsrsUploadDataSource(context, pattern, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSource(this ICakeContext context, string pattern, string folderPath, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadDataSource(context, pattern, folderPath, new Dictionary<string, string>(), settings);
        }

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settingsConfigurator">Connection Settings Configurator</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSource(this ICakeContext context, string pattern, string folderPath, IDictionary<string, string> properties, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            return SsrsUploadDataSource(context, pattern, folderPath, properties, settings);
        }

        /// <summary>
        /// Uploads an SSRS DataSource (.rds) to the folder specified for the given globber pattern
        /// </summary>
        /// <param name="context">Cake Contex</param>
        /// <param name="pattern">The filePath to rds file to upload</param>
        /// <param name="folderPath">The relative path to the SSRS to deploy the report to.</param>
        /// <param name="properties">A collection of properties to apply to the report</param>
        /// <param name="settings">Connection Settings</param>
        /// <returns><seealso cref="IEnumerable{CatalogItem}"/>Collection Catalog Items</returns>
        [CakeMethodAlias]
        [CakeAliasCategory("DataSources")]
        public static IEnumerable<CatalogItem> SsrsUploadDataSource(this ICakeContext context, string pattern, string folderPath, IDictionary<string, string> properties, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentNullException(nameof(folderPath));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var items = new List<CatalogItem>();

            var filePaths = context.Globber.GetFiles(pattern);
            if (filePaths == null || filePaths.Count() < 1)
            {
                context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Warning, "No Shared DataSet files found matching the pattern '{0}'", pattern);
                return items;
            }

            var client = GetReportingService(context, settings);

            foreach (var filePath in filePaths)
                items.Add(UploadCatalogItem(context, client, ItemType.DataSource, filePath, folderPath, properties));

            return items;
        }

        #endregion

        #region SsrsFindItem

        /// <summary>
        /// Finds a catalog item within the SSRS folder structure
        /// </summary>
        /// <param name="context">Cake Context</param>
        /// <param name="settingsConfigurator">SSRS Settings Action</param>
        /// <param name="request">Find ItemRequest Parameters.</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        public static CatalogItem SsrsFindItem(this ICakeContext context, FindItemRequest request, Action<SsrsConnectionSettings> settingsConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));
            
            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);
            
            return SsrsFindItem(context, request, settings);
        }

        /// <summary>
        /// Finds a catalog item within the SSRS folder structure
        /// </summary>
        /// <example>
        /// <code>
        ///           var catalogItem = SsrsFindItem( 
        ///                new FindItemRequest
        ///                {
        ///                     Folder = "/AdventureWorks",
        ///                     ItemName = "My_Report_Name",
        ///                     Recursive = false
        ///                },
        ///                new SsrsConnectionSettings
        ///                {
        ///                    ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        ///                    UseDefaultCredentials = true
        ///                });
        /// </code>        
        /// </example>
        /// <param name="context">Cake Context</param>
        /// <param name="settings">SSRS Settings</param>
        /// <param name="request">Find ItemRequest Parameters.</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        public static CatalogItem SsrsFindItem(this ICakeContext context, FindItemRequest request, SsrsConnectionSettings settings)
        {
            VerifyParameters(context, settings);

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Folder))
                throw new ArgumentNullException(nameof(request.Folder));

            if (string.IsNullOrWhiteSpace(request.ItemName))
                throw new ArgumentNullException(nameof(request.ItemName));

            return FindItem(context, GetReportingService(context, settings), settings, request);
        }

        #endregion

        private static CatalogItem FindItem(ICakeContext context, ReportingService client, SsrsConnectionSettings settings, FindItemRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var criteria = new FindItemsRequest
            {
                BooleanOperator = BooleanOperatorEnum.And,
                Folder = request.Folder,
                SearchConditions = new SearchCondition[]
                {
                 new SearchCondition
                 {
                     Name = "Name",
                     Values = new string[] {  request.ItemName },
                     Condition = ConditionEnum.Equals,
                     ConditionSpecified = true
                 }
                },
                SearchOptions = new Property[]
                 {
                    new Property
                    {
                        Name = "Resursive",
                        Value =  request.Recursive.ToString()
                    }
                 }
            };

            return client.FindItemsAsync(criteria).GetAwaiter().GetResult().Items?.FirstOrDefault();
        }

        private static CatalogItem UploadCatalogItem(ICakeContext context, ReportingService client, ItemType itemType, FilePath filePath, string folderPath, IDictionary<string, string> properties)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            if (filePath == null)
                throw new ArgumentException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException(nameof(folderPath));

            var request = new SaveItemRequest
            {
                ItemFilePath = filePath,
                ItemType = itemType,
                FolderPath = folderPath,
                Properties = properties?.Select(kvp => new Property { Name = kvp.Key, Value = kvp.Value }).ToArray()
            };

            switch (itemType)
            {
                case ItemType.Report:
                case ItemType.DataSet:
                    return CreateOrUpdateCatalogItem(context, client, request);
                case ItemType.DataSource:
                    return CreateOrUpdateDataSource(context, client, request);
                default:
                    throw new System.NotImplementedException($"The itemType { itemType } is currently not supported.");
            }
        }

        private static CatalogItem CreateOrUpdateCatalogItem(ICakeContext context, ReportingService client, SaveItemRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Property[] properties = null;
            if (request.Properties != null && request.Properties.Any())
                properties = request.Properties.Where(c => !ReadOnlyProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase)).ToArray();

            var file = context.FileSystem.GetFile(request.ItemFilePath);

            var fileFullPath = request.ItemFilePath.MakeAbsolute(context.Environment).FullPath;
            var itemName = System.IO.Path.GetFileNameWithoutExtension(fileFullPath);
            var buffer = System.IO.File.ReadAllBytes(fileFullPath);

            context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Information, "Uploading {0} to {1}...", itemName, request.FolderPath);

            var catalogItemRequest = new CreateCatalogItemRequest
            {
                Name = itemName,
                Definition = buffer,
                Parent = request.FolderPath,
                ItemType = request.ItemType.ToString(),
                Overwrite = true,
                Properties = properties
            };

            var catalogResponse = client.CreateCatalogItemAsync(catalogItemRequest).GetAwaiter().GetResult();

            UpdateItemReferences(client, catalogResponse);

            if (catalogResponse.Warnings != null && catalogResponse.Warnings.Any())
                foreach (var warning in catalogResponse.Warnings)
                    context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Warning, warning.Message);

            return catalogResponse.ItemInfo;
        }

        private static CatalogItem CreateOrUpdateDataSource(ICakeContext context, ReportingService client, SaveItemRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Property[] properties = null;
            if (request.Properties != null && request.Properties.Any())
                properties = request.Properties.Where(c => !ReadOnlyProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase)).ToArray();

            var file = context.FileSystem.GetFile(request.ItemFilePath);

            var fileFullPath = request.ItemFilePath.MakeAbsolute(context.Environment).FullPath;

            var serializer = new XmlSerializer(typeof(RptDataSource));

            var rds = (RptDataSource)serializer.Deserialize(new System.IO.FileStream(fileFullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read));

            var dsd = new DataSourceDefinition()
            {
                ConnectString = rds.ConnectionProperties?.ConnectString,
                Extension = rds.ConnectionProperties?.Extension
            };

            if (rds.ConnectionProperties?.IntegratedSecurity == true)
                dsd.CredentialRetrieval = CredentialRetrievalEnum.Integrated;
            
            context.Log.Write(Core.Diagnostics.Verbosity.Normal, Core.Diagnostics.LogLevel.Information, "Uploading {0} to {1}...", rds.Name, request.FolderPath);

            var dataSourceRequest = new CreateDataSourceRequest
            {
               DataSource = rds.Name,
               Parent = request.FolderPath,
               Definition = dsd,
               Overwrite = true,
               Properties = properties
            };

            var catalogResponse = client.CreateDataSourceAsync(dataSourceRequest).GetAwaiter().GetResult();
            return catalogResponse?.ItemInfo;
        }

        private static void UpdateItemReferences(ReportingService client, CreateCatalogItemResponse catalogResponse)
        {
            var itemReferencesResponse = client.GetItemReferencesAsync(new GetItemReferencesRequest { ItemPath = catalogResponse.ItemInfo.Path, ReferenceItemType = ItemType.DataSet.ToString() }).GetAwaiter().GetResult();

            if (itemReferencesResponse?.ItemReferences != null)
            {
                var newReferences = new List<ItemReference>();

                foreach (var itemReference in itemReferencesResponse.ItemReferences)
                {
                    var itemReferenceType = client.GetItemTypeAsync(new GetItemTypeRequest { ItemPath = itemReference.Reference }).GetAwaiter().GetResult()?.Type;
                    var newItemReference = new ItemReference
                    {
                        Name = itemReference.Name,
                        Reference = itemReference.Reference
                    };

                    newReferences.Add(newItemReference);
                }

                if (newReferences.Any())
                    client.SetItemReferencesAsync(new SetItemReferencesRequest { ItemPath = catalogResponse.ItemInfo.Path, ItemReferences = newReferences.ToArray() }).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Gets the reporting Service client need to 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <returns>Reporting Service 2010 Client</returns>
        private static ReportingService GetReportingService(ICakeContext context, SsrsConnectionSettings settings)
        {
            var url = new Uri(settings.ServiceEndpoint);

            var clientAuthType = GetClientCredential(settings.ClientCredentialType);
            var proxyClientType = GetProxyCredentialType(settings.ProxyCredentialType);

            HttpBindingBase binding = null;

            if (!string.Equals(url.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                var basicSecurityMode = GetHttpSecurityMode(settings.SecurityMode);
                var httpBinding = new BasicHttpBinding
                {
                    MaxBufferPoolSize = int.MaxValue,
                    CloseTimeout = TimeSpan.MaxValue,
                    MaxBufferSize = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,
                    OpenTimeout = TimeSpan.MaxValue,
                    ReceiveTimeout = TimeSpan.MaxValue,
                    SendTimeout = TimeSpan.MaxValue,
                    TextEncoding = Encoding.UTF8,
                    AllowCookies = true,
                    UseDefaultWebProxy = true
                };

                httpBinding.Security.Mode = basicSecurityMode;
                httpBinding.Security.Transport.ClientCredentialType = clientAuthType;
                httpBinding.Security.Transport.ProxyCredentialType = proxyClientType;

                binding = httpBinding;
            }
            else
            {
                var httpsBinding = new BasicHttpsBinding
                {
                    MaxBufferPoolSize = int.MaxValue,
                    CloseTimeout = TimeSpan.MaxValue,
                    MaxBufferSize = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,
                    OpenTimeout = TimeSpan.MaxValue,
                    ReceiveTimeout = TimeSpan.MaxValue,
                    SendTimeout = TimeSpan.MaxValue,
                    TextEncoding = Encoding.UTF8,
                    AllowCookies = true,
                    UseDefaultWebProxy = true
                };

                httpsBinding.Security.Mode = BasicHttpsSecurityMode.Transport;
                httpsBinding.Security.Transport.ClientCredentialType = clientAuthType;
                httpsBinding.Security.Transport.ProxyCredentialType = proxyClientType;

                binding = httpsBinding;
            }

            var client = new ReportingServiceClient(binding, new EndpointAddress(settings.ServiceEndpoint));

            switch (clientAuthType)
            {
                case HttpClientCredentialType.Basic:
                    client.ClientCredentials.UserName.UserName = !string.IsNullOrWhiteSpace(settings.Domain) ? $@"{settings.Domain}\{settings.Username}" : settings.Username;
                    client.ClientCredentials.UserName.Password = settings.Password;
                    break;
                case HttpClientCredentialType.Ntlm:
                case HttpClientCredentialType.Windows:
                    client.ClientCredentials.Windows.AllowedImpersonationLevel = settings.ImperonsationLevel;
                    client.ClientCredentials.Windows.ClientCredential = settings.UseDefaultCredentials ?
                                                                                        client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials
                                                                                        : new System.Net.NetworkCredential(settings.Username, settings.Password, settings.Password);
                    break;
            }

            return client;
        }

        private static void VerifyParameters(ICakeContext context, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
        }

        private static HttpClientCredentialType GetClientCredential(ClientCredentialType clientCredentialType)
        {            
            switch (clientCredentialType)
            {
                case ClientCredentialType.Basic:
                    return HttpClientCredentialType.Basic;
                case ClientCredentialType.Ntlm:
                    return HttpClientCredentialType.Ntlm;
                case ClientCredentialType.Windows:
                    return HttpClientCredentialType.Windows;
                default:
                    return HttpClientCredentialType.None;
            }
        }
        
        private static BasicHttpSecurityMode GetHttpSecurityMode(SecurityMode securityMode)
        {
            switch (securityMode)
            {
                case SecurityMode.Transport:
                    return BasicHttpSecurityMode.Transport;
                case SecurityMode.TransportCredentialOnly:
                    return BasicHttpSecurityMode.TransportCredentialOnly;
                case SecurityMode.TransportWithMessageCredential:
                    return BasicHttpSecurityMode.TransportWithMessageCredential;
                default:
                    return BasicHttpSecurityMode.None;
            }
        }

        private static HttpProxyCredentialType GetProxyCredentialType(ProxyCredentialType proxyCredentialType)
        {
            switch (proxyCredentialType)
            {
                case ProxyCredentialType.Basic:
                    return HttpProxyCredentialType.Basic;
                case ProxyCredentialType.Ntlm:
                    return HttpProxyCredentialType.Ntlm;
                case ProxyCredentialType.Windows:
                    return HttpProxyCredentialType.Windows;
                case ProxyCredentialType.None:
                default:
                    return HttpProxyCredentialType.None;
            }
        }
    }
}
