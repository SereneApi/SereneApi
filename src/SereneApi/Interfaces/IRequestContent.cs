using System;
using System.Linq.Expressions;

namespace SereneApi.Interfaces
{
    public interface IRequestContent : IRequestCreated
    {
        IRequestCreated WithInBodyContent<TContent>(TContent content);

        IRequestCreated WithQuery<TQueryable>(TQueryable queryable);
        IRequestCreated WithQuery<TQueryable>(TQueryable queryable, Expression<Func<TQueryable, object>> queryExpression);
    }
}
