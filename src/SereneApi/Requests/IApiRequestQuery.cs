using System;
using System.Linq.Expressions;
using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Queries;

namespace SereneApi.Requests
{
    public interface IApiRequestQuery : IApiRequestContent
    {
        /// <summary>
        /// Generates a query for the request based on the object supplied.
        /// </summary>
        /// <param name="query">The parameters to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <remarks>The query items are converted into <see cref="string"/>s using the <see cref="ObjectToStringFormatter"/>.</remarks>
        IApiRequestContent WithQuery<TQuery>(TQuery query);

        /// <summary>
        /// Generates a query for the request based on the object supplied.
        /// </summary>
        /// <param name="query">The parameters to be used.</param>
        /// <param name="selector">Enabled creation of a dynamic <see cref="object"/> to select specific properties of the query <see cref="object"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <remarks>The query items are converted into <see cref="string"/>s using the <see cref="ObjectToStringFormatter"/>.</remarks>
        IApiRequestContent WithQuery<TQuery>(TQuery query, Expression<Func<TQuery, object>> selector);
    }
}
