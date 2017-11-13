using Cake.Core;
using Cake.Core.Annotations;
using Cake.SSRS.Services;
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
        #region SsrsFindItem

        /// <summary>
        /// Finds a catalog item within the SSRS folder structure
        /// </summary>
        /// <param name="context">Cake Context</param>
        /// <param name="settingsConfigurator">SSRS Settings Action</param>
        /// <param name="requestConfigurator">Find Item Action.</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        public static CatalogItem SsrsFindItem(this ICakeContext context, Action<SsrsConnectionSettings> settingsConfigurator, Action<FindItemRequest> requestConfigurator)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));

            if (requestConfigurator == null)
                throw new ArgumentNullException(nameof(requestConfigurator));

            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);

            var request = new FindItemRequest();
            requestConfigurator(request);

            return SsrsFindItem(context, settings, request);
        }

        /// <summary>
        /// Finds a catalog item within the SSRS folder structure
        /// </summary>
        /// <param name="context">Cake Context</param>
        /// <param name="settingsConfigurator">SSRS Settings Action</param>
        /// <param name="requestConfigurator">Find Item Action.</param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        public static CatalogItem SsrsFindItem(this ICakeContext context, Action<SsrsConnectionSettings> settingsConfigurator, FindItemRequest request)
        {
            if (settingsConfigurator == null)
                throw new ArgumentNullException(nameof(settingsConfigurator));
            
            var settings = new SsrsConnectionSettings();
            settingsConfigurator(settings);
            
            return SsrsFindItem(context, settings, request);
        }

        /// <summary>
        /// Finds a catalog item within the SSRS folder structure
        /// </summary>
        /// <param name="context">Cake Context</param>
        /// <param name="settings">SSRS Settings</param>
        /// <param name="folder">The top level folder being the search.</param>
        /// <param name="itemName">The name of the item to find</param>
        /// <param name="recursive">true to search subfolder. False to only search </param>
        /// <returns><seealso cref="CatalogItem"/>Catalog Item</returns>
        [CakeMethodAlias]
        public static CatalogItem SsrsFindItem(this ICakeContext context, SsrsConnectionSettings settings, FindItemRequest request)
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

        private static CatalogItem FindItem(ICakeContext context, ReportingService2010Soap client, SsrsConnectionSettings settings, FindItemRequest request)
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

        /// <summary>
        /// Gets the reporting Service client need to 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <returns>Reporting Service 2010 Client</returns>
        private static ReportingService2010Soap GetReportingService(ICakeContext context, SsrsConnectionSettings settings)
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

            var client = new ReportingService2010SoapClient(httpBinding, new EndpointAddress(settings.ServiceEndpoint));

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
