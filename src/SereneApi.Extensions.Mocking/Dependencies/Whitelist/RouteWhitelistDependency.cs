using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Dependencies.Whitelist
{
    /// <summary>
    /// Only replies to requests that contain the specified route.
    /// </summary>
    public class RouteWhitelistDependency: IWhitelist
    {
        private readonly Uri[] _routes;

        /// <summary>
        /// Creates a new instance of <seealso cref="MethodWhitelistDependency"/>.
        /// </summary>
        /// <param name="routes">The routes that this request will reply to.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        public RouteWhitelistDependency([NotNull] params Uri[] routes)
        {
            if(routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }

            if(routes.Length <= 0)
            {
                throw new ArgumentException($"{nameof(routes)} must not be empty.");
            }

            _routes = routes;
        }

        /// <inheritdoc cref="IWhitelist.Validate"/>
        public Validity Validate(object value)
        {
            if(value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if(!(value is Uri route))
            {
                return Validity.NotApplicable;
            }

            if(_routes.Contains(route))
            {
                return Validity.Valid;
            }

            return Validity.Invalid;
        }
    }
}
