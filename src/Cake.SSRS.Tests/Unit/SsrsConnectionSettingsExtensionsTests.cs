using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cake.SSRS.Tests.Unit
{
    public sealed class SsrsConnectionSettingsExtensionsTests
    {
        public sealed class TheUseDefaultCredientialsMethod
        {
            [Fact]
            public void Should_Throw_On_Null_Settings_Parameter()
            {
                //Given                
                SsrsConnectionSettings settings = null;

                //When
                var nullRecord = Record.Exception(() => SsrsConnectionSettingsExtensions.UseDefaultCredientials(settings));

                //Then
                CakeAssert.IsArgumentNullException(nullRecord, nameof(settings));
            }

            [Fact]
            public void Should_Set_UseDefaultCredentials_Property_To_True()
            {
                //Given
                var settings = new SsrsConnectionSettings();

                //When
                settings.UseDefaultCredientials();

                //Then
                Assert.True(settings.UseDefaultCredentials);
                Assert.Null(settings.Domain);
                Assert.Null(settings.Username);
                Assert.Null(settings.Password);
            }
        }

        public sealed class TheAuthenticateWithMethod
        {
            [Fact]
            public void Should_Throw_On_Null_Settings_Parameter()
            {
                //Given                
                SsrsConnectionSettings settings = null;

                //When
                var nullRecord = Record.Exception(() => SsrsConnectionSettingsExtensions.UseDefaultCredientials(settings));

                //Then
                CakeAssert.IsArgumentNullException(nullRecord, nameof(settings));
            }

            [Fact]
            public void Should_Throw_On_Null_Username_Parameter()
            {
                //Given                
                SsrsConnectionSettings settings = new SsrsConnectionSettings();

                string userName = string.Empty;
                string password = "Password1";
                string domain = "MICROSOFT";

                //When
                var nullRecord = Record.Exception(() => SsrsConnectionSettingsExtensions.AuthenticateWith(settings, userName, password, domain));

                //Then
                CakeAssert.IsArgumentNullException(nullRecord, nameof(userName));
            }

            [Fact]
            public void Should_Throw_On_Null_Password_Parameter()
            {
                //Given                
                SsrsConnectionSettings settings = new SsrsConnectionSettings();

                string userName = "drwatson";
                string password = string.Empty;
                string domain = "MICROSOFT";

                //When
                var nullRecord = Record.Exception(() => SsrsConnectionSettingsExtensions.AuthenticateWith(settings, userName, password, domain));

                //Then
                CakeAssert.IsArgumentNullException(nullRecord, nameof(password));
            }

            [Fact]
            public void Should_Set_Credentials_Properties()
            {
                //Given
                SsrsConnectionSettings settings = new SsrsConnectionSettings();

                var userName = "drwatson";
                var password = "Password1";
                var domain = "MICROSOFT";

                //When
                settings.AuthenticateWith(userName, password, domain);

                //Then
                Assert.False(settings.UseDefaultCredentials);
                Assert.Equal(domain, settings.Domain);
                Assert.Equal(userName, settings.Username);
                Assert.Equal(password, settings.Password);
            }
        }

        public sealed class TheSetServiceEndpointMethod
        {
            [Fact]
            public void Should_Throw_On_Null_Settings_Parameter()
            {
                //Given                
                SsrsConnectionSettings settings = null;
                string serviceEndpoint = "";

                //When
                var nullRecord = Record.Exception(() => SsrsConnectionSettingsExtensions.SetServiceEndpoint(settings, serviceEndpoint));

                //Then
                CakeAssert.IsArgumentNullException(nullRecord, nameof(settings));
            }

            [Fact]
            public void Should_Throw_On_Null_ServiceEndpoint_Parameter()
            {
                //Given                
                SsrsConnectionSettings settings = new SsrsConnectionSettings();
                string serviceEndpoint = "";

                //When
                var record = Record.Exception(() => SsrsConnectionSettingsExtensions.SetServiceEndpoint(settings, serviceEndpoint));

                //Then
                CakeAssert.IsArgumentNullException(record, nameof(serviceEndpoint));
            }

            [Fact]
            public void Should_Throw_On_Invalid_ServiceEndpoint_Url_Parameter()
            {
                //Given                
                SsrsConnectionSettings settings = new SsrsConnectionSettings();
                string serviceEndpoint = "ftp://localhost/reportserver/ReportService2010.asmx";

                //When
                var record = Record.Exception(() => SsrsConnectionSettingsExtensions.SetServiceEndpoint(settings, serviceEndpoint));

                //Then
                CakeAssert.IsExceptionWithMessage<UriFormatException>(record, $"{ nameof(serviceEndpoint) } is not a valid http or https scheme url.");
            }

            [Fact]
            public void Should_Set_ServiceEndpoint_Url()
            {
                //Given                
                SsrsConnectionSettings settings = new SsrsConnectionSettings();
                string serviceEndpoint = "http://localhost/reportserver/ReportService2010.asmx";

                //When
                SsrsConnectionSettingsExtensions.SetServiceEndpoint(settings, serviceEndpoint);

                //Then
                Assert.Equal(serviceEndpoint, settings.ServiceEndpoint);
            }
        }
    }
}
