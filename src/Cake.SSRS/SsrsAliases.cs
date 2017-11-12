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
        public static CatalogItem FindItem(ICakeContext context, SsrsSettings settings, string folder, string itemName, bool recursive = false)
        {
            VerifyParameters(context, settings);

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
        private static ReportingService2010Soap GetReportingService(ICakeContext context, SsrsSettings settings)
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

        private static void VerifyParameters(ICakeContext context, SsrsSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
        }
    }
}
