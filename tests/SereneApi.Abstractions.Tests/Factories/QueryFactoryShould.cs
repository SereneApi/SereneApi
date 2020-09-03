using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Tests.Mock;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Abstractions.Tests.Factories
{
    public class QueryFactoryShould
    {
        private readonly IQueryFactory _queryFactory;

        public QueryFactoryShould()
        {
            IDefaultApiConfiguration configuration = DefaultApiConfiguration.Default;

            IApiOptionsBuilder apiOptionsBuilder = configuration.GetOptionsBuilder();

            apiOptionsBuilder.SetSource("http://test.com");

            IApiOptions options = apiOptionsBuilder.BuildOptions();

            _queryFactory = options.RetrieveRequiredDependency<IQueryFactory>();
        }

        [Fact]
        public void BuildQuery()
        {
            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15)
            };

            string query = _queryFactory.Build(personDto);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=2000-05-15");
        }

        [Fact]
        public void BuildQueryWithDateTime()
        {
            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = _queryFactory.Build(personDto);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=2000-05-15 05:35:20");
        }

        [Fact]
        public void BuildQueryWithNoQuerySection()
        {
            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = _queryFactory.Build(personDto, o => new { });

            query.ShouldBeNull();
        }

        [Fact]
        public void BuildQueryWithOneQuerySection()
        {
            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = _queryFactory.Build(personDto, o => new { o.Age });

            query.ShouldBe("?Age=18");
        }

        [Fact]
        public void BuildQueryWithTwoQuerySection()
        {
            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = _queryFactory.Build(personDto, o => new { o.Age, o.Name });

            query.ShouldBe("?Age=18&Name=John Smith");
        }
    }
}
