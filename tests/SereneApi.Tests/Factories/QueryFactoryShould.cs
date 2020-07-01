using SereneApi.Abstractions.Factories;
using SereneApi.Tests.Mock;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Tests.Factories
{
    public class QueryFactoryShould
    {
        [Fact]
        public void BuildQuery()
        {
            IQueryFactory queryFactory = new DefaultQueryFactory();

            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15)
            };

            string query = queryFactory.Build(personDto);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=2000-05-15");
        }

        [Fact]
        public void BuildQueryWithDateTime()
        {
            IQueryFactory queryFactory = new DefaultQueryFactory();

            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(personDto);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=2000-05-15 05:35:20");
        }

        [Fact]
        public void BuildQueryWithNoQuerySection()
        {
            IQueryFactory queryFactory = new DefaultQueryFactory();

            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(personDto, o => new { });

            query.ShouldBeNull();
        }

        [Fact]
        public void BuildQueryWithOneQuerySection()
        {
            IQueryFactory queryFactory = new DefaultQueryFactory();

            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(personDto, o => new { o.Age });

            query.ShouldBe("?Age=18");
        }

        [Fact]
        public void BuildQueryWithTwoQuerySection()
        {
            IQueryFactory queryFactory = new DefaultQueryFactory();

            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(personDto, o => new { o.Age, o.Name });

            query.ShouldBe("?Age=18&Name=John Smith");
        }

        [Fact]
        public void BuildQueryUsingCustomFormatter()
        {
            IQueryFactory queryFactory = new DefaultQueryFactory(CustomQueryFormatter);

            MockPersonDto personDto = new MockPersonDto
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(personDto);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=15-05-2000");
        }

        private static string CustomQueryFormatter(object queryObject)
        {
            if(queryObject is DateTime dateTimeQuery)
            {
                return dateTimeQuery.ToString("dd-MM-yyyy");
            }

            return queryObject.ToString();
        }
    }
}
