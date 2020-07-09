using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Mocking.Dependencies.Whitelist
{
    /// <summary>
    /// Validates the request before responding.
    /// </summary>
    public interface IWhitelist
    {
        /// <summary>
        /// Validates the <see cref="IWhitelist"/> item.
        /// </summary>
        /// <remarks>If the value is not of the correct type NotApplicable should be returned.</remarks>
        /// <param name="value">The value to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        Validity Validate([NotNull] object value);
    }
}
