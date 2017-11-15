using Cake.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    /// <summary>
    /// SSRS Item Type
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// SSRS Report (.rdl) file
        /// </summary>
        Report,
        /// <summary>
        /// Shared DataSet (.rsd) file
        /// </summary>
        DataSet,
        /// <summary>
        /// Shared DataSource (.rds) file
        /// </summary>
        DataSource
    }
}