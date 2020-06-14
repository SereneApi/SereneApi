using System;
using System.Linq.Expressions;

namespace SereneApi.Interfaces
{
    public interface IRequestContent : IRequestCreated
    {
        IRequestCreated AddInBodyContent<TContent>(TContent content);

        IRequestCreated AddQuery<TQueryable>(TQueryable queryable);
        IRequestCreated AddQuery<TQueryable>(TQueryable queryable, Expression<Func<TQueryable, object>> queryExpression);
    }
}
