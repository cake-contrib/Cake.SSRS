using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using NSubstitute;
using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace Cake.SSRS.Tests.Fixtures
{
    public sealed class CakeContextFixture : ICakeContext, IDisposable
    {
        public IFileSystem FileSystem { get; set; }
        public ICakeEnvironment Environment { get; set; }
        public IGlobber Globber { get; set; }
        public ICakeLog Log { get; set; }
        public ICakeArguments Arguments { get; set; }
        public IProcessRunner ProcessRunner { get; set; }
        public IRegistry Registry { get; set; }
        public IToolLocator Tools { get; set; }
        public ICakeDataResolver Data { get; set; }
        public ICakeConfiguration Configuration { get; set; }

        public string ReportsDirectory { get; set; }
        public string DataSetsDirectory { get; set; }
        public string DataSourcesDirectory { get; set; }

        public bool IsRunningOnAppVeyor { get; private set; }

        public string ServiceEndpoint { get; private set; }

        public CakeContextFixture()
        {
            IsRunningOnAppVeyor = string.Equals(System.Environment.GetEnvironmentVariable("APPVEYOR"), "true", StringComparison.OrdinalIgnoreCase);

            ServiceEndpoint = !IsRunningOnAppVeyor ? "http://localhost/reportserver/ReportService2010.asmx" : "http://appvyr-win/Reports_SQL2014/reportserver/ReportService2010.asmx";

            var cakeRuntime = Substitute.For<ICakeRuntime>();

            cakeRuntime.BuiltFramework.Returns(new FrameworkName(".NET Framework", new Version(4, 6, 1)));
            cakeRuntime.CakeVersion.Returns(typeof(ICakeRuntime).GetTypeInfo().Assembly.GetName().Version);

            Log = Substitute.For<ICakeLog>();
            Arguments = Substitute.For<ICakeArguments>();
            ProcessRunner = Substitute.For<IProcessRunner>();
            Registry = Substitute.For<IRegistry>();
            Tools = Substitute.For<IToolLocator>();
            FileSystem = Substitute.For<IFileSystem>();

            Environment = Substitute.For<ICakeEnvironment>();

            Environment.Runtime.Returns(cakeRuntime);
            Environment.WorkingDirectory.Returns(new DirectoryPath(AppContext.BaseDirectory));

            Configuration = Substitute.For<ICakeConfiguration>();

            Globber = Substitute.For<IGlobber>();


            Globber.GetFiles(Arg.Is<string>("./App_Data/**/Emp*.rsd")).Returns(new FilePath[]
            {
                new FilePath("./App_Data/DataSets/EmployeeSalesDetail.rsd"),
                new FilePath("./App_Data/DataSets/EmpSalesMonth.rsd"),
                new FilePath("./App_Data/DataSets/EmployeeSalesYearOverYear.rsd")
            });

            Globber.GetFiles(Arg.Is<string>("./App_Data/**/*.rdl")).Returns(new FilePath[]
            {
                new FilePath("./App_Data/Reports/Company Sales.rdl"),
                new FilePath("./App_Data/Reports/Sales Order Detail.rdl"),
                new FilePath("./App_Data/Reports/Store_Contacts.rdl")
            });

            Globber.GetFiles(Arg.Is<string>("./App_Data/**/*.rds")).Returns(new FilePath[]
            {
                new FilePath("./App_Data/DataSources/AdventureWorks.rds")
            });

            ReportsDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "App_Data", "Reports");
            DataSetsDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "App_Data", "DataSets");
            DataSourcesDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "App_Data", "DataSources");
        }

        public void Dispose()
        { }
    }
}
