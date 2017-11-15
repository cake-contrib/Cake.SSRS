using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

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

        #endregion

        #region SsrsUploadReport

        /// <summary>
        /// Uploads an SSRS Report (.rdl) to the folder specified
        /// </summary>
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

            return CreateOrUpdateCatalogItem(context, client, request);
        }

        private static CatalogItem CreateOrUpdateCatalogItem(ICakeContext context, ReportingService client, SaveItemRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));


            Property[] properties = null;

            var file = context.FileSystem.GetFile(request.ItemFilePath);

            var fileFullPath = request.ItemFilePath.MakeAbsolute(context.Environment).FullPath;
            var itemName = System.IO.Path.GetFileNameWithoutExtension(fileFullPath);
            var buffer = System.IO.File.ReadAllBytes(fileFullPath);

            if (request.Properties != null && request.Properties.Any())
                properties = request.Properties.Where(c => !ReadOnlyProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase)).ToArray();

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
            var httpBinding = new BasicHttpBinding()
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

            httpBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            httpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

            var client = new ReportingServiceClient(httpBinding, new EndpointAddress(settings.ServiceEndpoint));

            if (settings.UseDefaultCredentials)
                client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            else
                client.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(settings.Username, settings.Password, settings.Password);

            return client;
        }

        private static void VerifyParameters(ICakeContext context, SsrsConnectionSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
        }
    }
}
