using System;
using System.Linq.Expressions;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestQuery : IApiRequestHeaders
    {
        IApiRequestHeaders WithQuery<TQuery>(TQuery query) where TQuery : class;

        IApiRequestHeaders WithQuery<TQuery>(TQuery query, Expression<Func<TQuery, object>> selector) where TQuery : class;

        IApiRequestHeaders WithQuery<TQuery>(Action<TQuery> builder) where TQuery : class;
    }
}