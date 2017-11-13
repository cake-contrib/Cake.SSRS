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
        public static CatalogItem SsrsFindItem(this ICakeContext context, SsrsConnectionSettings settings, string folder, string itemName, bool recursive = false)
        {
            VerifyParameters(context, settings);

            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentNullException(nameof(folder));

            if (string.IsNullOrWhiteSpace(itemName))
                throw new ArgumentNullException(nameof(itemName));

            var client = GetReportingService(context, settings);

            var request = new FindItemsRequest
            {
                BooleanOperator = BooleanOperatorEnum.And,
                Folder = folder,
                SearchConditions = new SearchCondition[]
                {
                 new SearchCondition
                 {
                     Name = "Name",
                     Values = new string[] { itemName },
                     Condition = ConditionEnum.Equals,
                     ConditionSpecified = true
                 }
                },
                SearchOptions = new Property[]
                 {
                    new Property
                    {
                        Name = "Resursive",
                        Value = recursive.ToString()
                    }
                 }
            };

            return client.FindItemsAsync(request).GetAwaiter().GetResult().Items?.FirstOrDefault();
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
