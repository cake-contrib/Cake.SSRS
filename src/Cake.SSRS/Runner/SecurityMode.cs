using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    /// <summary>
    /// Security Mode Specificed
    /// </summary>
    public enum SecurityMode
    {
        /// <summary>
        /// No security mode specified. Only valid for Http 
        /// </summary>
        None,
        /// <summary>
        /// SSL Certicate Htpp Authenication
        /// </summary>
        Transport,
        /// <summary>
        /// Provider only HTTP Authentication
        /// </summary>
        TransportCredentialOnly,
        /// <summary>
        /// 
        /// </summary>           
        TransportWithMessageCredential
    }
}
