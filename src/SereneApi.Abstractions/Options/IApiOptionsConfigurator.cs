using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authentication;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Serializers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsConfigurator
    {
        /// <summary>
        /// The source that requests will be made against.
        /// </summary>
        /// <param name="source">The source of the host.</param>
        /// <param name="resource">The api resource.</param>
        /// <param name="resourcePath">The api resource path, appended before the resource.</param>
        /// <remarks>
        /// <para>By default the resource path is set to "api/". To disable the default set the value to an empty string.</para>
        /// <para>If a resource is not provided it can be supplied when making requests by using the AgainstResource method.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void UseSource([NotNull] string source, [AllowNull] string resource = null, [AllowNull] string resourcePath = null);

        /// <summary>
        /// Sets the timeout period.
        /// </summary>
        /// <param name="seconds">The time in seconds.</param>
        /// <remarks>By defaults request will timeout in 30 seconds.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when a value of 0 or less is provided.</exception>
        void SetTimeout([NotNull] int seconds);

        /// <summary>
        /// Sets the timeout period.
        /// </summary>
        /// <param name="seconds">The time in seconds.</param>
        /// <param name="attempts">How many attempts will be made before the request times out. An attempt count of 0 can be provided.</param>
        /// <remarks>
        /// <para>By defaults request will timeout in 30 seconds.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when a value of 0 or less is provided.</exception>
        void SetTimeout([NotNull] int seconds, [NotNull] int attempts);

        /// <summary>
        /// Sets the amount of times the request will be re-attempted before failing.
        /// </summary>
        /// <param name="attempts">How many attempts will be made before the request times out.</param>
        /// <remarks>By defaults requests will not be re-attempted.</remarks>
        /// /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// /// <exception cref="ArgumentException">Thrown when a value is below 0.</exception>
        void SetRetryAttempts([NotNull] int attempts);

        void AddLogger([NotNull] ILogger logger);

        void UseQueryFactory([NotNull] IQueryFactory queryFactory);

        void UseRouteFactory([NotNull] IRouteFactory routeFactory);

        void UseSerializer([NotNull] ISerializer serializer);

        void AddAuthentication([NotNull] IAuthentication authentication);

        void UseCredentials([NotNull] ICredentials credentials);

        void AddBasicAuthentication(string username, string password);

        void AddBearerAuthentication(string token);

        void AcceptContentType(ContentType content);
    }
}
