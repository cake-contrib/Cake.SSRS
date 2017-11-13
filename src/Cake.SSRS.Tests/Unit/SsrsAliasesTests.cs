using Cake.Core;
using Cake.SSRS.Tests.Fixtures;
using System;
using System.Linq;
using Xunit;

namespace Cake.SSRS.Tests.Unit
{
    public sealed class SsrsAliasesTests
    {
        private const string ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx";

        [Collection(Traits.CakeContextCollection)]
        public sealed class TheSsrsFindItemMethod
        {
            private readonly ICakeContext _Context;
            private readonly SsrsConnectionSettings _Settings;

            public TheSsrsFindItemMethod(CakeContextFixture fixture)
            {
                _Context = fixture;
                _Settings = new SsrsConnectionSettings
                {
                     ServiceEndpoint = ServiceEndpoint,
                     UseDefaultCredentials = true
                };
            }

            [Fact]
            public void Should_Throw_On_Null_Settings_Context()
            {
                //Given                
                ICakeContext context = null;
                SsrsConnectionSettings settings = _Settings;
                FindItemRequest request = new FindItemRequest
                {
                    Folder = "Folder Url",
                    ItemName = "Item Name",
                    Recursive = true
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, settings, request));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(context));
            }

            [Fact]
            public void Should_Throw_On_Null_Settings_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = null;
                FindItemRequest request = new FindItemRequest
                {
                    Folder = "Folder Url",
                    ItemName = "Item Name",
                    Recursive = true
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, settings, request));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(settings));
            }

            [Fact]
            public void Should_Throw_On_Null_Or_Empty_Folder_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                FindItemRequest request = new FindItemRequest
                {
                    Folder = null,
                    ItemName = "Item Name",
                    Recursive = true
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, settings, request));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(request.Folder));
            }

            [Fact]
            public void Should_Throw_On_Null_Or_Empty_ItemName_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                FindItemRequest request = new FindItemRequest
                {
                    Folder = "/AdventureWorks",
                    ItemName = null,
                    Recursive = true
                };


                //When
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, settings, request));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(request.ItemName));
            }

            [Fact]
            public void Should_Throw_On_Null_Or_Empty_Request_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                FindItemRequest request = null;


                //When
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, settings, request));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(request));
            }

            [Theory]
            [InlineData("/AdventureWorks", "Employee_Sales_Summary")]
            public void Should_Return_Catalog_Item(string folder, string itemName)
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                FindItemRequest request = new FindItemRequest
                {
                    Folder = folder,
                    ItemName = itemName,
                    Recursive = true
                };

                //When
                var item = SsrsAliases.SsrsFindItem(context, settings, request);

                //Then
                Assert.NotNull(item);
            }
        }
    }
}
