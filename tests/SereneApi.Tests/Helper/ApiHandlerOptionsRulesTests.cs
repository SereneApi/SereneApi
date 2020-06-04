using SereneApi.Helpers;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Tests.Helper
{
    public class ApiHandlerOptionsRulesTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void ValidRetryCountPass(uint count)
        {
            Should.NotThrow(() =>
            {
                ApiHandlerOptionsRules.ValidateRetryCount(count);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void ValidRetryCountFail(uint count)
        {
            Should.Throw<ArgumentException>(() =>
            {
                ApiHandlerOptionsRules.ValidateRetryCount(count);
            });
        }
    }
}
