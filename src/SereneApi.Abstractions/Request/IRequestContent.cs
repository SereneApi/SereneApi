using SereneApi.Abstractions.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace SereneApi.Abstractions.Request
{
    public interface IRequestContent: IRequestCreated
    {
        /// <summary>
        /// Adds the specified content into the body of the request.
        /// </summary>
        /// <typeparam name="TContent">The type of the content.</typeparam>
        /// <param name="content">The content to be appended to the body of the request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestCreated AddInBodyContent<TContent>([NotNull] TContent content);

        /// <summary>
        /// Adds the specified content into the body of the request.
        /// </summary>
        /// <param name="content">The content to be appended to the body of the request.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestCreated AddInBodyContent([NotNull] IApiRequestContent content);

        /// <summary>
        /// The specified object is appended to the request as a query.
        /// </summary>
        /// <typeparam name="TQueryable">The query value type.</typeparam>
        /// <param name="queryable">The value to be appended as a query.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestCreated WithQuery<TQueryable>([NotNull] TQueryable queryable);
        /// <summary>
        /// The specified object is appended to the request as a query.
        /// </summary>
        /// <typeparam name="TQueryable">The query value type.</typeparam>
        /// <param name="queryable">The value to be appended as a query.</param>
        /// <param name="queryExpression">Selects parts of the value to be used in the query.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestCreated WithQuery<TQueryable>([NotNull] TQueryable queryable, [NotNull] Expression<Func<TQueryable, object>> queryExpression);
    }
}
