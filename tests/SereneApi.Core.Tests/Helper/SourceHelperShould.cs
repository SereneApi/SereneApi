using SereneApi.Core.Helpers;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Core.Tests.Helper
{
    public class SourceHelperShould
    {
        [Theory]
        [InlineData("http://localhost")]
        [InlineData("http://localhost/api")]
        [InlineData("http://localhost/api/resource")]
        public void CheckIfValidStringFailTests(string source)
        {
            Should.Throw<ArgumentException>(() =>
            {
                SourceHelpers.CheckIfValid(source);
            });
        }

        [Theory]
        [InlineData("http://localhost/")]
        [InlineData("http://localhost/api/")]
        [InlineData("http://localhost/api/resource/")]
        public void CheckIfValidStringPassTests(string source)
        {
            Should.NotThrow(() =>
            {
                SourceHelpers.CheckIfValid(source);
            });
        }

        [Theory]
        [InlineData("http://localhost/api")]
        [InlineData("http://localhost/api/resource")]
        public void CheckIfValidUriFailTests(string source)
        {
            Uri sourceUri = new Uri(source);

            Should.Throw<ArgumentException>(() =>
            {
                SourceHelpers.CheckIfValid(sourceUri);
            });
        }

        [Theory]
        // This looks like it should be wrong, but a uri with the host only will append a / to the
        // end even if the string value does not.
        [InlineData("http://localhost")]
        [InlineData("http://localhost/")]
        [InlineData("http://localhost/api/")]
        [InlineData("http://localhost/api/resource/")]
        public void CheckIfValidUriPassTests(string source)
        {
            Uri sourceUri = new Uri(source);

            Should.NotThrow(() =>
            {
                SourceHelpers.CheckIfValid(sourceUri);
            });
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("http://localhost", "http://localhost/")]
        [InlineData("http://localhost/api", "http://localhost/api/")]
        [InlineData("http://localhost/api/resource", "http://localhost/api/resource/")]
        public void EnsureSourceSlashTerminationString(string source, string expectedSource)
        {
            source = SourceHelpers.EnsureSlashTermination(source);

            source.ShouldBe(expectedSource);
        }

        [Theory]
        [InlineData("http://localhost", "http://localhost/")]
        [InlineData("http://localhost/api", "http://localhost/api/")]
        [InlineData("http://localhost/api/resource", "http://localhost/api/resource/")]
        public void EnsureSourceSlashTerminationUri(string source, string expectedSource)
        {
            Uri sourceUri = new Uri(source);
            Uri expectedSourceUri = new Uri(expectedSource);

            sourceUri = SourceHelpers.EnsureSlashTermination(sourceUri);

            sourceUri.ShouldBe(expectedSourceUri);
        }
    }
}