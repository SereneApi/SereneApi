using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Authorization;
using SereneApi.Core.Connection;
using SereneApi.Core.Content;
using SereneApi.Core.Requests.Handler;
using SereneApi.Core.Responses.Handlers;
using SereneApi.Core.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SereneApi.Core.Options.Factories
{
    public interface IApiOptionsFactory
    {
        /// <summary>
        /// The content must be of the specific type.
        /// </summary>
        /// <param name="type">The type the response must be if it is to be accepted.</param>
        void AcceptContentType(ContentType type);

        /// <summary>
        /// Specifies an <see cref="IAuthorization"/> which will be used when authenticating.
        /// </summary>
        /// <param name="authorization">The <see cref="IAuthorization"/> to be for authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddAuthentication(IAuthorization authorization);

        /// <summary>
        /// Adds basic authentication to used when making requests.
        /// </summary>
        /// <param name="username">The username to authenticate with.</param>
        /// <param name="password">The password to authenticate with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddBasicAuthentication(string username, string password);

        /// <summary>
        /// Adds bearer authentication to used when making requests.
        /// </summary>
        /// <param name="token">The token to authenticate with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddBearerAuthentication(string token);

        /// <summary>
        /// Adds the APIs connection information using the provided <see cref="IConnectionSettings"/>.
        /// </summary>
        /// <param name="configuration">
        /// The configuration that will be used for communication with the API.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when the method is called twice.</exception>
        void AddConfiguration(IConnectionSettings connectionSettings);

        /// <summary>
        /// Specifies an <see cref="ICredentials"/> which will be used when authenticating.
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to be for authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void AddCredentials(ICredentials credentials);

        /// <summary>
        /// Sets the amount of times the request will be re-attempted before failing.
        /// </summary>
        /// <param name="attempts">How many attempts will be made before the request times out.</param>
        /// <remarks>By defaults requests will not be re-attempted.</remarks>
        /// <exception cref="ArgumentException">Thrown when a value is below 0.</exception>
        void SetRetryAttempts(int attempts);

        /// <summary>
        /// The source that requests will be made against.
        /// </summary>
        /// <param name="baseAddress">The address of the host.</param>
        /// <param name="resource">The api resource.</param>
        /// <param name="resourcePath">The api resource path, appended before the resource.</param>
        /// <remarks>
        /// <para>
        /// By default the resource path is set to "api/". To disable the default set the value to
        /// an empty string.
        /// </para>
        /// <para>
        /// If a resource is not provided it can be supplied when making requests by using the
        /// AgainstResource method.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void SetSource(string baseAddress, [AllowNull] string resource = null, [AllowNull] string resourcePath = null);

        /// <summary>
        /// Sets the timeout period.
        /// </summary>
        /// <param name="seconds">The time in seconds.</param>
        /// <remarks>By defaults request will timeout in 30 seconds.</remarks>
        /// <exception cref="ArgumentException">Thrown when a value of 0 or less is provided.</exception>
        /// <exception cref="MethodAccessException">
        /// Thrown when this is called prior to a source being provided.
        /// </exception>
        void SetTimeout(int seconds);

        /// <summary>
        /// Sets the timeout period.
        /// </summary>
        /// <param name="seconds">The time in seconds.</param>
        /// <param name="attempts">
        /// How many attempts will be made before the request times out. An attempt count of 0 can
        /// be provided.
        /// </param>
        /// <remarks>
        /// <para>By defaults request will timeout in 30 seconds.</para>
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when a value of 0 or less is provided.</exception>
        /// <exception cref="MethodAccessException">
        /// Thrown when this is called prior to a source being provided.
        /// </exception>
        void SetTimeout(int seconds, int attempts);

        /// <summary>
        /// All exceptions generated by SereneApi will be thrown.
        /// </summary>
        void ThrowExceptions();

        /// <summary>
        /// Specifies the <see cref="IFailedResponseHandler"/> to be used for handling failed response.
        /// </summary>
        /// <param name="handler">
        /// The <see cref="IFailedResponseHandler"/> to be used for handling failed responses.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown whe a null value is provided.</exception>
        void UseFailedResponseHandler(IFailedResponseHandler handler);

        /// <summary>
        /// Specifies the <see cref="IFailedResponseHandler"/> to be used for handling failed response.
        /// </summary>
        /// <param name="handlerBuilder">
        /// The <see cref="IFailedResponseHandler"/> to be used for handling failed responses.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown whe a null value is provided.</exception>
        void UseFailedResponseHandler(Func<IDependencyProvider, IFailedResponseHandler> handlerBuilder);

        /// <summary>
        /// Specifies an <see cref="ILogger"/> to be used for logging.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to be used for logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void UseLogger(ILogger logger);

        /// <summary>
        /// Specifies the <see cref="IRequestHandler"/> to be used for handling requests.
        /// </summary>
        /// <param name="handler">The <see cref="IRequestHandler"/> to be used for handling requests.</param>
        /// <exception cref="ArgumentNullException">Thrown whe a null value is provided.</exception>
        void UseRequestHandler(IRequestHandler handler);

        /// <summary>
        /// Specifies the <see cref="IRequestHandler"/> to be used for handling requests.
        /// </summary>
        /// <param name="handlerBuilder">
        /// The <see cref="IRequestHandler"/> to be used for handling requests.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown whe a null value is provided.</exception>
        void UseRequestHandler(Func<IDependencyProvider, IRequestHandler> handlerBuilder);

        /// <summary>
        /// Specifies the <see cref="IResponseHandler"/> to be used for handling response.
        /// </summary>
        /// <param name="handler">The <see cref="IResponseHandler"/> to be used for handling responses.</param>
        /// <exception cref="ArgumentNullException">Thrown whe a null value is provided.</exception>
        void UseResponseHandler(IResponseHandler handler);

        /// <summary>
        /// Specifies the <see cref="IResponseHandler"/> to be used for handling response.
        /// </summary>
        /// <param name="handlerBuilder">
        /// The <see cref="IResponseHandler"/> to be used for handling responses.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown whe a null value is provided.</exception>
        void UseResponseHandler(Func<IDependencyProvider, IResponseHandler> handlerBuilder);

        /// <summary>
        /// Specifies an <see cref="ISerializer"/> to be used for serializing requests and
        /// deserializing responses.
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer"/> to be used for serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        void UseSerializer(ISerializer serializer);
    }
}