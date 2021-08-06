﻿using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Connection;
using SereneApi.Abstractions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using SereneApi.Abstractions.Options.Types;

namespace SereneApi.Extensions.DependencyInjection.Options
{
    /// <inheritdoc cref="IApiOptions{TApi}"/>
    internal class ApiOptions<TApi> : ApiOptions, IApiOptions<TApi> where TApi : class
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApiOptions{TApi}"/>
        /// </summary>
        /// <param name="dependencies">The dependencies that can be used.</param>
        /// <param name="connection">The <see cref="IConnectionSettings"/> used to make requests to the API.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public ApiOptions([NotNull] IDependencyProvider dependencies, [NotNull] IConnectionSettings connection) : base(dependencies, connection)
        {
        }
    }
}