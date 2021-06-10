using System;
using System.Linq.Expressions;

namespace SereneApi.Requests
{
    public interface IApiRequestQuery : IApiRequestContent
    {
        IApiRequestContent WithQuery<TQuery>(TQuery query);
        IApiRequestContent WithQuery<TQuery>(TQuery query, Expression<Func<TQuery, object>> selector);
    }
}
