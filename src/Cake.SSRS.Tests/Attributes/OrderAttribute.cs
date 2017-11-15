using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS.Tests
{
    /// <summary>
    /// Allows ordering of Xunit tests
    /// </summary>
    public class OrderAttribute : Attribute
    {
        public int I { get; }

        /// <summary>
        /// Order Attribute
        /// </summary>
        /// <param name="i"></param>
        public OrderAttribute(int i)
        {
            I = i;
        }
    }
}
