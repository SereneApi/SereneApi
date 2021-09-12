using SereneApi.Core.Connection;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Core.Tests.Connection
{
    public class ConnectionSettingsShould
    {
        [Theory]
        [InlineData("http://localhost/", "Users", "api/")]
        [InlineData("http://localhost/", "Users/", "api")]
        [InlineData("http://localhost/", "User/Pictures", "api/")]
        [InlineData("http://localhost/", "User/Pictures/", "api")]
        [InlineData("http://localhost/", "User", "services/api/")]
        [InlineData("http://localhost/", "User/", "services/api")]
        public void ShouldThrowArgumentException(string source, string resource, string resourcePath)
        {
            Should.Throw<ArgumentException>(() => new ConnectionSettings(source, resource, resourcePath));
        }
    }
}