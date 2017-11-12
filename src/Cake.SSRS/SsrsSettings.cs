using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    /// <summary>
    /// Settings for the SSRS Report server and Report Objects to deploy
    /// </summary>
    public class SsrsSettings
    {
        /// <summary>
        /// Gets or Sets the url to the ReportService2010 endpoint
        /// </summary>
        /// <example>
        /// http://<Server Name>/ReportServer/ReportService2010.asmx
        /// </example>
        public string ServiceEndpoint { get; set; }

        /// <summary>
        ///The Username used to connect to the SSRS Server
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The Password used to connect to the SSRS Server
        /// </summary>
        public string Password { get; set; }

        public string Domain { get; set; }

        /// <summary>
        /// Determines whether to use the default credentials of the person logged in.
        /// Ignores userName and Password if set to true
        /// </summary>
        public bool UseDefaultCredentials { get; set; }
    }
}
