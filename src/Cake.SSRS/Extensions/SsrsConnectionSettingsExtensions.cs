using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    /// <summary>
    /// Containts functionality related to SSRS Settings
    /// </summary>
    public static class SsrsConnectionSettingsExtensions
    {
        /// <summary>
        /// Uses the default credentials of the context the process it currently running in to authenticate to the SSRS server.
        /// </summary>
        /// <param name="settings">The Settings</param>
        /// <returns>The same <see cref="SsrsConnectionSettings"/> instance so that multiple calls can be chained.</returns>
        public static SsrsConnectionSettings UseDefaultCredientials(this SsrsConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            settings.Password = null;
            settings.Domain = null;
            settings.Username = null;

            settings.UseDefaultCredentials = true;

            return settings;
        }

        /// <summary>
        /// Configures the settings to use to authenticate with to the SSRS server.
        /// </summary>
        /// <param name="settings">The Settings</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <param name="domain">Domain the username belongs to.</param>
        /// <returns>The same <see cref="SsrsConnectionSettings"/> instance so that multiple calls can be chained.</returns>
        public static SsrsConnectionSettings AuthenticateWith(this SsrsConnectionSettings settings, string userName, string password, string domain)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            settings.Username = userName;
            settings.Password = password;
            settings.Domain = domain ?? string.Empty;
            
            settings.UseDefaultCredentials = false;

            return settings;
        }

        /// <summary>
        /// Configures the settings to use to authenticate with to the SSRS server.
        /// </summary>
        /// <param name="settings">The Settings</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns>The same <see cref="SsrsConnectionSettings"/> instance so that multiple calls can be chained.</returns>
        public static SsrsConnectionSettings AuthenticateWith(this SsrsConnectionSettings settings, string userName, string password)
        {
            return AuthenticateWith(settings, userName, password, string.Empty);
        }

        /// <summary>
        /// Sets the Service Enpoint to connect to SSRS with
        /// </summary>
        /// <param name="settings">The Settings</param>
        /// <param name="serviceEndpoint">The url of the endpoint to use.</param>
        /// <returns>The same <see cref="SsrsConnectionSettings"/> instance so that multiple calls can be chained.</returns>
        public static SsrsConnectionSettings SetServiceEndpoint(this SsrsConnectionSettings settings, string serviceEndpoint)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(serviceEndpoint))
                throw new ArgumentNullException(nameof(serviceEndpoint));

            if (!ValidUrl(serviceEndpoint))
                throw new UriFormatException($"{ nameof(serviceEndpoint) } is not a valid http or https scheme url.");

            settings.ServiceEndpoint = serviceEndpoint;

            return settings;
        }

        private static bool ValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
