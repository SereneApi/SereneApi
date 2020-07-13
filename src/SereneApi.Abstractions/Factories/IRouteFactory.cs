using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Factories
{
    /// <summary>
    /// Builds a route using the supplied values.
    /// </summary>
    public interface IRouteFactory
    {
        /// <summary>
        /// The resource path appended to the route.
        /// </summary>
        public string ResourcePath { get; }

        /// <summary>
        /// Specifies the resource to be used in the route.
        /// </summary>
        /// <param name="resource">The resource to be used in the route.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddResource([NotNull] string resource);

        /// <summary>
        /// Specifies the query to be appended to the end of the route.
        /// </summary>
        /// <param name="queryString">The query to be appended to the route.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddQuery([NotNull] string queryString);

        /// <summary>
        /// Specifies the parameters to be appended to the route.
        /// </summary>
        /// <param name="parameters">The parameters to be appended to the route.</param>
        /// <remarks>
        /// A single parameter can be applied with ot without an end point.
        /// If more than one parameter is provided a formattable end point is required.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when more than one parameter was provided without a formattable end point.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddParameters([NotNull] params object[] parameters);

        /// <summary>
        /// Supplies the End Point to be appended to the route.
        /// </summary>
        /// <param name="endpoint">The endpoint to be appended to the route.</param>
        /// <remarks>
        /// The end point can be a formattable string. If this is done parameters will be required.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddEndpoint([NotNull] string endpoint);

        /// <summary>
        /// The route will be built and returned as an <see cref="Uri"/>.
        /// </summary>
        /// <param name="clearSettings">Clears the previous settings.</param>
        Uri BuildRoute(bool clearSettings = true);

        /// <summary>
        /// Clears the provided values.
        /// </summary>
        void Clear();
    }
}
