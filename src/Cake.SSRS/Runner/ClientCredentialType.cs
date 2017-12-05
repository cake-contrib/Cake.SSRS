using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    /// <summary>
    /// Client CredentialType used to authenticate against SSRS
    /// </summary>
    public enum ClientCredentialType
    {
        /// <summary>
        /// No Authentication required
        /// </summary>
        None = 4,
        /// <summary>
        /// Basic Authentication
        /// </summary>
        Basic = 2,
        /// <summary>
        /// NTLM Authentication
        /// </summary>
        Ntlm = 1,
        /// <summary>
        /// Windows Authentication
        /// </summary>
        Windows = 0,
    }
}
