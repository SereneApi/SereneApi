using SereneApi.Core.Configuration;
using SereneApi.Core.Options.Factories;
using SereneApi.Handlers.Rest.Queries;
using SereneApi.Handlers.Rest.Tests.Mock;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Handlers.Rest.Tests.Factories
{
    public class QueryFactoryShould
    {
        private readonly IQueryFactory _queryFactory;

        public QueryFactoryShould()
        {
            ConfigurationManager configuration = new ConfigurationManager();

            ApiOptionsFactory<BaseApiHandlerWrapper> optionsFactory = configuration.BuildApiOptionsFactory<BaseApiHandlerWrapper>();

            _queryFactory = optionsFactory.Dependencies.BuildProvider().GetDependency<IQueryFactory>();
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

            query.ShouldBe("?Age=18&BirthDate=2000-05-15&Name=John Smith");
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

            query.ShouldBe("?Age=18&BirthDate=2000-05-15 05:35:20&Name=John Smith");
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