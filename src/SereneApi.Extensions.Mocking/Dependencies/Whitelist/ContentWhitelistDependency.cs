using SereneApi.Core.Content;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Dependencies.Whitelist
{
    /// <summary>
    /// Only replies to requests that contain the specified <see cref="IApiRequestContent"/>.
    /// </summary>
    public class ContentWhitelistDependency : IWhitelist
    {
        private readonly IRequestContent[] _whitelistedContent;

        /// <summary>
        /// Creates a new instance of <see cref="ContentWhitelistDependency"/>.
        /// </summary>
        /// <param name="content">The content that this request will reply to.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        public ContentWhitelistDependency([NotNull] params IRequestContent[] content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (content.Length <= 0)
            {
                throw new ArgumentException($"{nameof(content)} must not be empty.");
            }

            _whitelistedContent = content;
        }

        /// <inheritdoc cref="IWhitelist.Validate"/>
        public Validity Validate([NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is IRequestContent content))
            {
                return Validity.NotApplicable;
            }

            if (_whitelistedContent.Contains(content))
            {
                return Validity.Valid;
            }

            return Validity.Invalid;
        }
    }
}
