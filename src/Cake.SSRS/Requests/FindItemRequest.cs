using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    /// <summary>
    /// SSRS Find Item criteria
    /// </summary>
    public class FindItemRequest
    {
        /// <summary>
        /// The top level folder being the search.
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// The name of the item to find
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// true to search subfolders. False to only search current folder.
        /// </summary>
        public bool Recursive { get; set; }
    }
}