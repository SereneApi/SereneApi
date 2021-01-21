using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Authorization;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Queries;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Routing;
using SereneApi.Abstractions.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// Configures <see cref="IApiOptions"/>.
    /// </summary>
    public interface IApiOptionsConfigurator
    {
        /// <summary>
        /// Adds the APIs connection information using the provided <see cref="IConnectionConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The configuration that will be used for communication with the API.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when the method is called twice.</exception>
        void AddConfiguration([NotNull] IConnectionConfiguration configuration);

        /// <summary>
        /// The source that requests will be made against.
        /// </summary>
        /// <param name="baseAddress">The address of the host.</param>
        /// <param name="resource">The api resource.</param>
        /// <param name="resourcePath">The api resource path, appended before the resource.</param>
        /// <remarks>
        /// <para>By default the resource path is set to "api/". To disable the default set the value to an empty string.</para>
        /// <para>If a resource is not provided it can be supplied when making requests by using the AgainstResource method.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void SetSource([NotNull] string baseAddress, [AllowNull] string resource = null, [AllowNull] string resourcePath = null);

        /// <summary>
        /// Sets the timeout period.
        /// </summary>
        /// <param name="seconds">The time in seconds.</param>
        /// <remarks>By defaults request will timeout in 30 seconds.</remarks>
        /// <exception cref="ArgumentException">Thrown when a value of 0 or less is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when this is called prior to a source being provided.</exception>
        void SetTimeout([NotNull] int seconds);

        /// <summary>
        /// Sets the timeout period.
        /// </summary>
        /// <param name="seconds">The time in seconds.</param>
        /// <param name="attempts">How many attempts will be made before the request times out. An attempt count of 0 can be provided.</param>
        /// <remarks>
        /// <para>By defaults request will timeout in 30 seconds.</para>
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when a value of 0 or less is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when this is called prior to a source being provided.</exception>
        void SetTimeout(int seconds, int attempts);

        /// <summary>
        /// Sets the amount of times the request will be re-attempted before failing.
        /// </summary>
        /// <param name="attempts">How many attempts will be made before the request times out.</param>
        /// <remarks>By defaults requests will not be re-attempted.</remarks>
        /// <exception cref="ArgumentException">Thrown when a value is below 0.</exception>
        void SetRetryAttempts(int attempts);

        /// <summary>
        /// Specifies an <see cref="ILogger"/> to be used for logging.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to be used for logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void UseLogger([NotNull] ILogger logger);

        /// <summary>
        /// Specifies an <see cref="IQueryFactory"/> to be when building queries.
        /// </summary>
        /// <param name="queryFactory">The <see cref="IQueryFactory"/> to be used for building queries.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void UseQueryFactory([NotNull] IQueryFactory queryFactory);

        /// <summary>
        /// Specifies an <see cref="IRouteFactory"/> to be when building routes.
        /// </summary>
        /// <param name="routeFactory">The <see cref="IRouteFactory"/> to be used for building routes.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void UseRouteFactory([NotNull] IRouteFactory routeFactory);

        /// <summary>
        /// Specifies an <see cref="ISerializer"/> to be used for serializing requests and deserializing responses.
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to be used for serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void UseSerializer([NotNull] ISerializer serializer);

        /// <summary>
        /// Specifies an <see cref="IAuthorization"/> which will be used when authenticating.
        /// </summary>
        /// <param name="authorization">The <see cref="IAuthorization"/> to be for authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddAuthentication([NotNull] IAuthorization authorization);

        /// <summary>
        /// Specifies an <see cref="ICredentials"/> which will be used when authenticating.
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to be for authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddCredentials([NotNull] ICredentials credentials);

        /// <summary>
        /// Adds basic authentication to used when making requests.
        /// </summary>
        /// <param name="username">The username to authenticate with.</param>
        /// <param name="password">The password to authenticate with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddBasicAuthentication([NotNull] string username, [NotNull] string password);

        /// <summary>
        /// Adds bearer authentication to used when making requests.
        /// </summary>
        /// <param name="token">The token to authenticate with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddBearerAuthentication([NotNull] string token);

        /// <summary>
        /// The content must be of the specific type.
        /// </summary>
        /// <param name="type">The type the response must be if it is to be accepted.</param>
        void AcceptContentType(ContentType type);
    }
}
