using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cake.SSRS.Tests
{
    /// <summary>
    /// These tests only succeed if you run all tests in the class.
    /// </summary>
    [TestCaseOrderer(TestCaseOrderer.TypeName, TestCaseOrderer.AssembyName)]
    public abstract class TestClassBase
    {
        protected static int I;

        protected void AssertTestName(string testName)
        {
            var type = GetType();
            var queue = TestCaseOrderer.QueuedTests[type.FullName];
            string dequeuedName;
            var result = queue.TryDequeue(out dequeuedName);

            Assert.True(result);
            Assert.Equal(testName, dequeuedName);
        }
    }
}