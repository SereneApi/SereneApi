using SereneApi.Abstractions.Requests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Dependencies.Whitelist
{
    /// <summary>
    /// Only replies to requests that contain the specified <see cref="Method"/>.
    /// </summary>
    public class MethodWhitelistDependency : IWhitelist
    {
        private readonly Method[] _method;

        /// <summary>
        /// Creates a new instance of <seealso cref="MethodWhitelistDependency"/>.
        /// </summary>
        /// <param name="methods">The <seealso cref="Method"/> that this request will reply to.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty or an invalid <see cref="Method"/> is provided.</exception>
        public MethodWhitelistDependency([NotNull] params Method[] methods)
        {
            if (methods == null)
            {
                throw new ArgumentNullException(nameof(methods));
            }

            if (methods.Length <= 0)
            {
                throw new ArgumentException($"{nameof(methods)} must not be empty.");
            }

            if (methods.Any(m => m == Method.NONE))
            {
                throw new ArgumentException("An invalid method was provided.");
            }

            _method = methods;
        }

        /// <inheritdoc cref="IWhitelist.Validate"/>
        public Validity Validate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is Method method))
            {
                return Validity.NotApplicable;
            }

            if (_method.Contains(method))
            {
                return Validity.Valid;
            }

            return Validity.Invalid;
        }
    }
}
