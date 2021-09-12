using System;
using System.Linq.Expressions;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestQuery : IApiRequestType
    {
        IApiRequestType WithQuery<TQuery>(TQuery query) where TQuery : class;

        IApiRequestType WithQuery<TQuery>(TQuery query, Expression<Func<TQuery, object>> selector) where TQuery : class;
    }
}