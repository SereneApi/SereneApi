using SereneApi.Core.Helpers;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Core.Tests.Helper
{
    public class RulesHelperShould
    {
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
    }
}