using System;
using System.Linq.Expressions;

namespace SereneApi.Interfaces
{
    public interface IApiRequestBuilder
    {
        void WithEndPoint(string endPoint);

        void WithEndPoint(object parameter);

        void WithEndPoint(string endpoint, params object[] parameters);

        void UsingMethod(Method method);

        void AddQuery<TQueryable>(TQueryable queryable, Expression<Func<TQueryable, object>> queryExpression);

        void AddInBodyContent<TContent>(TContent content);
    }
}
