using SereneApi.Factories;
using SereneApi.Interfaces;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Tests.Factories
{
    public class QueryFactoryTests
    {
        private class QueryObject
        {
            public int Age { get; set; }

            public string Name { get; set; }

            public DateTime BirthDate { get; set; }
        }

        [Fact]
        public void BuildQuery()
        {
            IQueryFactory queryFactory = new QueryFactory();

            QueryObject queryObject = new QueryObject
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15)
            };

            string query = queryFactory.Build(queryObject);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=2000-05-15");
        }

        [Fact]
        public void BuildQueryWithDateTime()
        {
            IQueryFactory queryFactory = new QueryFactory();

            QueryObject queryObject = new QueryObject
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(queryObject);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=2000-05-15 05:35:20");
        }

        [Fact]
        public void BuildQueryWithNoQuerySection()
        {
            IQueryFactory queryFactory = new QueryFactory();

            QueryObject queryObject = new QueryObject
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            Should.Throw<ArgumentException>(() =>
            {
                queryFactory.Build(queryObject, o => new { });
            });
        }

        [Fact]
        public void BuildQueryWithOneQuerySection()
        {
            IQueryFactory queryFactory = new QueryFactory();

            QueryObject queryObject = new QueryObject
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(queryObject, o => new { o.Age });

            query.ShouldBe("?Age=18");
        }

        [Fact]
        public void BuildQueryWithTwoQuerySection()
        {
            IQueryFactory queryFactory = new QueryFactory();

            QueryObject queryObject = new QueryObject
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(queryObject, o => new { o.Age, o.Name });

            query.ShouldBe("?Age=18&Name=John Smith");
        }

        [Fact]
        public void BuildQueryUsingCustomFormatter()
        {
            IQueryFactory queryFactory = new QueryFactory(CustomQueryFormatter);

            QueryObject queryObject = new QueryObject
            {
                Age = 18,
                Name = "John Smith",
                BirthDate = new DateTime(2000, 05, 15, 05, 35, 20)
            };

            string query = queryFactory.Build(queryObject);

            query.ShouldBe("?Age=18&Name=John Smith&BirthDate=15-05-2000");
        }

        private static string CustomQueryFormatter(object queryObject)
        {
            if (queryObject is DateTime dateTimeQuery)
            {
                return dateTimeQuery.ToString("dd-MM-yyyy");
            }

            return queryObject.ToString();
        }
    }
}
