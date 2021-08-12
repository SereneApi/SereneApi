using Shouldly;
using System;
using SereneApi.Core.Helpers;
using Xunit;

namespace SereneApi.Tests.Helper
{
    public class ApiHandlerOptionsRulesTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void ValidRetryCountPass(int count)
        {
            Should.NotThrow(() =>
            {
                Rules.ValidateRetryAttempts(count);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void ValidRetryCountFail(int count)
        {
            Should.Throw<ArgumentException>(() =>
            {
                Rules.ValidateRetryAttempts(count);
            });
        }
    }
}
