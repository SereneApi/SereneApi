using System;
using System.Linq.Expressions;

namespace DeltaWare.SereneApi.Interfaces
{
    public interface IQueryFactory
    {
        string Build<TQueryable>(TQueryable query);

        string Build<TQueryable>(TQueryable query, Expression<Func<TQueryable, object>> selector);

    }
}
