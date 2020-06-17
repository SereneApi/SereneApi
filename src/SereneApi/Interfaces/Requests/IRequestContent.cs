using System;
using System.Linq.Expressions;

namespace SereneApi.Interfaces.Requests
{
    public interface IRequestContent: IRequestCreated
    {
        /// <summary>
        /// Adds the supplied value into the body of the request.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> of the value to be added to the body of the request.</typeparam>
        /// <param name="content">The content to be added to the body of the request.</param>
        IRequestCreated WithInBodyContent<TContent>(TContent content);

        /// <summary>
        /// Adds a query to the request using all public properties of the supplied value.
        /// </summary>
        /// <typeparam name="TQueryable">The <see cref="Type"/> of the <b>queryable</b>.</typeparam>
        /// <param name="queryable">The <see cref="object"/> used to construct the query.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        IRequestCreated WithQuery<TQueryable>(TQueryable queryable);
        /// <summary>
        /// Adds a query to the request using the specified anonymous type to create the query.
        /// </summary>
        /// <typeparam name="TQueryable">The <see cref="Type"/> of the <b>queryable</b>.</typeparam>
        /// <param name="queryable">The <see cref="object"/> used to construct the query.</param>
        /// <param name="queryExpression">The expression used to select what public properties of the type will be used to build the query.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        IRequestCreated WithQuery<TQueryable>(TQueryable queryable, Expression<Func<TQueryable, object>> queryExpression);
    }
}
