using Cake.Core;
using Cake.Core.IO;
using Cake.SSRS.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cake.SSRS.Tests.Unit
{
    public sealed class SsrsAliasesTests
    {   
        private const string ParentFolderPath = "AdventureWorks";

        [Collection(Traits.CakeContextCollection)]
        [Order(1)]
        public sealed class TheCreateFolderMethod : TestClassBase
        {
            private readonly ICakeContext _Context;
            private readonly SsrsConnectionSettings _Settings;

            public TheCreateFolderMethod(CakeContextFixture fixture)
            {
                _Context = fixture;
                _Settings = new SsrsConnectionSettings
                {
                    ServiceEndpoint = fixture.ServiceEndpoint,
                    UseDefaultCredentials = true
                };
            }

            [Fact]
            public void Should_Throw_On_Null_FolderName_Context()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                string folderName = string.Empty;
                string parentFolder = null;

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsCreateFolder(context, folderName, parentFolder, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(folderName));
            }

            [Fact(Skip = "Integration Test Does not work on AppVeyor")]
            [Order(1)]
            [Trait(Traits.TestCategory, TestCategory.Integration)]
            public void Should_Create_New_Folder()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                string folderName = ParentFolderPath;
                string parentFolder = null;

                //When
                var item = SsrsAliases.SsrsCreateFolder(context, folderName, parentFolder, settings);

                //Then
                Assert.NotNull(item);
                Assert.Equal(item.Name, folderName, ignoreCase: true, ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
            }
        }

        [Collection(Traits.CakeContextCollection)]
        [Order(1)]
        public sealed class TheRemoveFolderMethod : TestClassBase
        {
            private readonly ICakeContext _Context;
            private readonly SsrsConnectionSettings _Settings;

            public TheRemoveFolderMethod(CakeContextFixture fixture)
            {
                _Context = fixture;
                _Settings = new SsrsConnectionSettings
                {
                    ServiceEndpoint = fixture.ServiceEndpoint,
                    UseDefaultCredentials = true
                };
            }

            [Fact]
            public void Should_Throw_On_Null_FolderName_Context()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                string folderName = string.Empty;
                string parentFolder = null;

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsRemoveFolder(context, folderName, parentFolder, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(folderName));
            }

            [Fact(Skip = "Integration Test Does not work on AppVeyor")]
            [Order(1)]
            [Trait(Traits.TestCategory, TestCategory.Integration)]
            public void Should_Create_New_Folder()
            {
                //Given                
                ICakeContext context = _Context;
                SsrsConnectionSettings settings = _Settings;
                string folderName = ParentFolderPath;
                string parentFolder = null;
                var item = SsrsAliases.SsrsCreateFolder(context, folderName, parentFolder, settings);
                Assert.NotNull(item);

                //When
                SsrsAliases.SsrsRemoveFolder(context, folderName, parentFolder, settings);

                //Then
                item = SsrsAliases.SsrsFindItem(context, new FindItemRequest { Folder = parentFolder, ItemName = folderName }, settings);
                Assert.Null(item);
            }
        }

        [Collection(Traits.CakeContextCollection)]
        [Order(2)]
        public sealed class TheUploadDataSourceMethod : TestClassBase
        {
            private readonly CakeContextFixture _Context;
            private readonly SsrsConnectionSettings _Settings;

            public TheUploadDataSourceMethod(CakeContextFixture fixture)
            {
                _Context = fixture;
                _Settings = new SsrsConnectionSettings
                {
                    ServiceEndpoint = fixture.ServiceEndpoint,
                    UseDefaultCredentials = true
                };
            }

            [Fact]
            public void Should_Throw_On_Null_Settings_Context()
            {
                //Given                
                ICakeContext context = null;
                SsrsConnectionSettings settings = _Settings;
                FilePath filePath = "./App_Data/DataSources/AdventureWorks.rds";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSource"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSource(context, filePath, folderPath, properties, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(context));
            }

            [Fact]
            public void Should_Throw_On_Null_SettingsConfigurator_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = null;
                FilePath filePath = "./App_Data/DataSources/AdventureWorks.rds";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSource"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSource(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(settingsConfigurator));
            }

            [Fact]
            public void Should_Throw_On_Null_FilePath_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = s => { };
                FilePath filePath = null;
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSource"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSet(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(filePath));
            }

            [Fact]
            public void Should_Throw_On_Null_FolderPath_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = s => { };
                FilePath filePath = "./App_Data/DataSources/AdventureWorks.rds";
                string folderPath = null;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSource(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(folderPath));
            }

            [Fact]
            public void Should_Throw_On_Null_Pattern_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                string pattern = null;
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSource"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSource(context, pattern, folderPath, properties, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(pattern));
            }

            [Fact]
            public void Should_Return_On_Empty_Collection_On_Not_Matching_Any_Files_For_Pattern()
            {
                //Given                
                ICakeContext context = _Context;
                string pattern = "App_Data/Foobar/*.rds";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSource"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var records = SsrsAliases.SsrsUploadDataSource(context, pattern, folderPath, properties, settings);

                //Then
                Assert.NotNull(records);
                Assert.Empty(records);
            }

            [Fact(Skip = "Integration Test Does not work on AppVeyor")]
            [Order(2)]
            public void Should_Upload_Single_DataSource()
            {
                //Given                
                ICakeContext context = _Context;
                FilePath pattern = System.IO.Path.Combine(_Context.DataSourcesDirectory, "AdventureWorks.rds");
                string folderPath = "/" + ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSource"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var record = SsrsAliases.SsrsUploadDataSource(context, pattern, folderPath, properties, settings);

                //Then
                Assert.NotNull(record);
                Assert.Equal("AdventureWorks", record.Name, ignoreCase: true, ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
            }
        }

        [Collection(Traits.CakeContextCollection)]
        [Order(3)]
        public sealed class TheUploadDataSetMethod : TestClassBase
        {
            private readonly CakeContextFixture _Context;
            private readonly SsrsConnectionSettings _Settings;

            public TheUploadDataSetMethod(CakeContextFixture fixture)
            {
                _Context = fixture;
                _Settings = new SsrsConnectionSettings
                {
                    ServiceEndpoint = fixture.ServiceEndpoint,
                    UseDefaultCredentials = true
                };
            }

            [Fact]
            public void Should_Throw_On_Null_Settings_Context()
            {
                //Given                
                ICakeContext context = null;
                SsrsConnectionSettings settings = _Settings;
                FilePath filePath = "./App_Data/Reports/SalesEmployees.rsd";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSet"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSet(context, filePath, folderPath, properties, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(context));
            }

            [Fact]
            public void Should_Throw_On_Null_SettingsConfigurator_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = null;
                FilePath filePath = "./App_Data/Reports/SalesEmployees.rsd";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSet"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSet(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(settingsConfigurator));
            }

            [Fact]
            public void Should_Throw_On_Null_FilePath_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = s => { };
                FilePath filePath = null;
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSet"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSet(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(filePath));
            }

            [Fact]
            public void Should_Throw_On_Null_FolderPath_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = s => { };
                FilePath filePath = "./App_Data/Reports/SalesEmployees.rsd";
                string folderPath = null;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSet(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(folderPath));
            }

            [Fact]
            public void Should_Throw_On_Null_Pattern_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                string pattern = null;
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSet"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadDataSet(context, pattern, folderPath, properties, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(pattern));
            }

            [Fact]
            public void Should_Return_On_Empty_Collection_On_Not_Matching_Any_Files_For_Pattern()
            {
                //Given                
                ICakeContext context = _Context;
                string pattern = "App_Data/Foobar/*.rsd";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSet"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var records = SsrsAliases.SsrsUploadDataSet(context, pattern, folderPath, properties, settings);

                //Then
                Assert.NotNull(records);
                Assert.Empty(records);
            }

            [Fact(Skip = "Integration Test Does not work on AppVeyor")]
            [Order(3)]
            [Trait(Traits.TestCategory, TestCategory.Integration)]
            public void Should_Upload_Single_DataSet()
            {
                //Given                
                ICakeContext context = _Context;
                FilePath pattern = System.IO.Path.Combine(_Context.DataSetsDirectory, "SalesEmployees.rsd");
                string folderPath = "/" + ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSet"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var record = SsrsAliases.SsrsUploadDataSet(context, pattern, folderPath, properties, settings);

                //Then
                Assert.NotNull(record);
                Assert.Equal("SalesEmployees", record.Name, ignoreCase: true, ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
            }

            [Fact(Skip = "Integration Test Does not work on AppVeyor")]
            [Order(3)]
            [Trait(Traits.TestCategory, TestCategory.Integration)]
            public void Should_Upload_Multiple_DataSets()
            {
                //Given                
                ICakeContext context = _Context;
                string pattern = "./App_Data/**/Emp*.rsd";
                string folderPath = "/" + ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a DataSet"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var records = SsrsAliases.SsrsUploadDataSet(context, pattern, folderPath, properties, settings);

                //Then
                Assert.NotEmpty(records);
                Assert.Equal(3, records.Count());
            }
        }

        [Collection(Traits.CakeContextCollection)]
        [Order(4)]
        public sealed class TheUploadReportMethod : TestClassBase
        {
            private readonly CakeContextFixture _Context;
            private readonly SsrsConnectionSettings _Settings;

            public TheUploadReportMethod(CakeContextFixture fixture)
            {
                _Context = fixture;
                _Settings = new SsrsConnectionSettings
                {
                    ServiceEndpoint = fixture.ServiceEndpoint,
                    UseDefaultCredentials = true
                };
            }

            [Fact]
            public void Should_Throw_On_Null_Settings_Context()
            {
                //Given                
                ICakeContext context = null;
                SsrsConnectionSettings settings = _Settings;
                FilePath filePath = "./App_Data/Reports/Employee_Sales_Summary.rdl";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadReport(context, filePath, folderPath, properties, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(context));
            }

            [Fact]
            public void Should_Throw_On_Null_SettingsConfigurator_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = null;
                FilePath filePath = "./App_Data/Reports/Employee_Sales_Summary.rdl";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadReport(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(settingsConfigurator));
            }

            [Fact]
            public void Should_Throw_On_Null_FilePath_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = s => { };
                FilePath filePath = null;
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadReport(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(filePath));
            }

            [Fact]
            public void Should_Throw_On_Null_FolderPath_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = s => { };
                FilePath filePath = "./App_Data/Reports/Employee_Sales_Summary.rdl";
                string folderPath = null;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadReport(context, filePath, folderPath, properties, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(folderPath));
            }

            [Fact]
            public void Should_Throw_On_Null_Pattern_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                string pattern = null;
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsUploadReport(context, pattern, folderPath, properties, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(pattern));
            }

            [Fact]
            public void Should_Return_On_Empty_Collection_On_Not_Matching_Any_Files_For_Pattern()
            {
                //Given                
                ICakeContext context = _Context;
                string pattern = "App_Data/Foobar/*.rdl";
                string folderPath = ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var records = SsrsAliases.SsrsUploadReport(context, pattern, folderPath, properties, settings);

                //Then
                Assert.NotNull(records);
                Assert.Empty(records);
            }

            [Fact(Skip = "Integration Test Does not work on AppVeyor")]
            [Order(4)]
            [Trait(Traits.TestCategory, TestCategory.Integration)]
            public void Should_Upload_Single_Report()
            {
                //Given                
                ICakeContext context = _Context;
                FilePath pattern = System.IO.Path.Combine(_Context.ReportsDirectory, "Employee_Sales_Summary.rdl");
                string folderPath = "/" + ParentFolderPath;
                IDictionary<string, string> properties = new Dictionary<string, string>()
                {
                    ["Description"] = "Great Description for a report"
                };
                SsrsConnectionSettings settings = _Settings;

                //When
                var record = SsrsAliases.SsrsUploadReport(context, pattern, folderPath, properties, settings);

                //Then
                Assert.NotNull(record);
                Assert.Equal("Employee_Sales_Summary", record.Name, ignoreCase: true, ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
            }
        }

        [Collection(Traits.CakeContextCollection)]
        [Order(5)]
        public sealed class TheSsrsFindItemMethod : TestClassBase
        {
            private readonly ICakeContext _Context;
            private readonly SsrsConnectionSettings _Settings;

            public TheSsrsFindItemMethod(CakeContextFixture fixture)
            {
                _Context = fixture;
                _Settings = new SsrsConnectionSettings
                {
                    ServiceEndpoint = fixture.ServiceEndpoint,
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
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, request, settings));

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
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, request, settings));

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
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, request, settings));

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
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, request, settings));

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
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, request, settings));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(request));
            }

            [Fact]
            public void Should_Throw_On_Null_SettingsConfigurator_Parameter()
            {
                //Given                
                ICakeContext context = _Context;
                Action<SsrsConnectionSettings> settingsConfigurator = null;
                FindItemRequest request = new FindItemRequest
                {
                    Folder = "/AdventureWorks",
                    ItemName = null,
                    Recursive = true
                };

                //When
                var record = Record.Exception(() => SsrsAliases.SsrsFindItem(context, request, settingsConfigurator));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(settingsConfigurator));
            }

            [Theory(Skip = "Integration Test Does not work on AppVeyor")]
            [InlineData("/AdventureWorks", "Employee_Sales_Summary")]
            [Order(5)]
            [Trait(Traits.TestCategory, TestCategory.Integration)]
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
                var item = SsrsAliases.SsrsFindItem(context, request, settings);

                //Then
                Assert.NotNull(item);
            }
        }
    }
}