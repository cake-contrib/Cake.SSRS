using Cake.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    /// <summary>
    /// Saves an item to the SSRS Server
    /// </summary>
    public class SaveItemRequest
    {
        /// <summary>
        /// Folder path on the SSRS server the item will be saved to
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// The path of the file to save to the SSRS Server
        /// </summary>
        public FilePath ItemFilePath { get; set; }

        /// <summary>
        /// Type of item being saved to SSRS
        /// </summary>
        public ItemType ItemType { get; set; }

        /// <summary>
        /// List of properties to apply to the item.
        /// Common ones include Name, DisplayName, Description, Hidden, etc.
        /// </summary>
        public Property[] Properties { get; set; }
    }
}
