# Cake.SSRS

Cake.SSRS is set of aliases for [Cake](http://cakebuild.net/) that help simplify deploying objects to SQL Server Reporting Services.
Release notes can be found [here](https://github.com/cake-contrib/Cake.SSRS/releases).

[![License](http://img.shields.io/:license-mit-blue.svg)](http://cake-contrib.mit-license.org)

## Information

| |Stable|Pre-release|
|:--:|:--:|:--:|
|GitHub Release|-|[![GitHub release](https://img.shields.io/github/release/cake-contrib/Cake.SSRS.svg)](https://github.com/cake-contrib/Cake.SSRS/releases/latest)|
|NuGet|[![NuGet](https://img.shields.io/nuget/v/Cake.SSRS.svg)](https://www.nuget.org/packages/Cake.SSRS)|[![NuGet](https://img.shields.io/nuget/vpre/Cake.SSRS.svg)](https://www.nuget.org/packages/Cake.SSRS)|

## Build Status

|Develop|Master|
|:--:|:--:|
|[![Build status](https://ci.appveyor.com/api/projects/status/dab3r83a6gkn7ycu/branch/develop?svg=true)](https://ci.appveyor.com/project/cakecontrib/cake-ssrs/branch/develop)|[![Build status](https://ci.appveyor.com/api/projects/status/dab3r83a6gkn7ycu/branch/master?svg=true)](https://ci.appveyor.com/project/cakecontrib/cake-ssrs/branch/master)|

## Code Coverage

[![Coverage Status](https://coveralls.io/repos/github/cake-contrib/Cake.SSRS/badge.svg?branch=develop)](https://coveralls.io/github/cake-contrib/Cake.SSRS?branch=develop)


## Chat Room

Come join in the conversation about Cake.SSRS in our Gitter Chat Room

[![Join the chat at https://gitter.im/cake-contrib/Lobby](https://badges.gitter.im/cake-contrib/Lobby.svg)](https://gitter.im/cake-contrib/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## About

This Addin only contains the functionality needed to upload the most common object types to SSRS (Reports, DataSets, and DataSources). Pull requests are accepted if you need additional functionality. Don't forget to include the unit tests with it.

## Usage  

### Creating a Folder

```csharp
SsrsCreateFolder("AdventureWorks", "/", new SsrsConnectionSettings
    {
        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        UseDefaultCredentials = true,
		ProxyCredentialType = ProxyCredentialType.Ntlm,
		ImperonsationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation,
		SecurityMode = SecurityMode.TransportCredentialOnly
    });
```

### Removing a Folder

```csharp
SsrsRemoveFolder("AdventureWorks", "/", new SsrsConnectionSettings
    {
        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        UseDefaultCredentials = true,
		ProxyCredentialType = ProxyCredentialType.Ntlm,
		ImperonsationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation,
		SecurityMode = SecurityMode.TransportCredentialOnly
    });
```

### Upload a Report

```csharp
SsrsUploadReport("./path/to/report.rdl", "/AdventureWorks",
    new Dictionary<string, string>
    {
            ["Description"] = "Description for the Report"
    },
    new SsrsConnectionSettings
    {
        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        UseDefaultCredentials = true,
		ProxyCredentialType = ProxyCredentialType.Ntlm,
		ImperonsationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation,
		SecurityMode = SecurityMode.TransportCredentialOnly
    });
```

### Upload a Shared DataSet

```csharp
SsrsUploadReport("./path/to/dataset.rsd", "/AdventureWorks",
    new Dictionary<string, string>
    {
            ["Description"] = "Description for the DataSet"
    },
    new SsrsConnectionSettings
    {
        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        UseDefaultCredentials = true,
		ProxyCredentialType = ProxyCredentialType.Ntlm,
		ImperonsationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation,
		SecurityMode = SecurityMode.TransportCredentialOnly

    });
```

### Upload a DataSource

```csharp
SsrsUploadDataSource("./path/to/datasource.rds", "/AdventureWorks",
    new Dictionary<string, string>
    {
            ["Description"] = "Description for the DataSource"
    },
    new SsrsConnectionSettings
    {
        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        UseDefaultCredentials = true,
		ProxyCredentialType = ProxyCredentialType.Ntlm,
		ImperonsationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation,
		SecurityMode = SecurityMode.TransportCredentialOnly
    });
```

### Search for an Item

```csharp
var catalogItem = SsrsFindItem ( 
    new FindItemRequest
    {
        Folder = "/AdventureWorks",
        ItemName = "My_Report_Name",
        Recursive = false
    },
    new SsrsConnectionSettings
    {
        ServiceEndpoint = "http://localhost/reportserver/ReportService2010.asmx",
        UseDefaultCredentials = true,
		ProxyCredentialType = ProxyCredentialType.Ntlm,
		ImperonsationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation,
		SecurityMode = SecurityMode.TransportCredentialOnly
    });
```