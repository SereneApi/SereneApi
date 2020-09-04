using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Request
{
    public interface IRequestEndPointParams: IRequestContent
    {
        /// <summary>
        /// Provides a formatted endpoint template which the request will be made against.
        /// </summary>
        /// <param name="parameters">The parameters that will be appended to the template.</param>
        /// <exception cref="ArgumentException">Thrown when no parameters are provided.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when this method is called an no endpoint has been provided.</exception>
        IRequestContent WithParameters([NotNull] params object[] parameters);

        /// <summary>
        /// Provides a formatted endpoint template which the request will be made against.
        /// </summary>
        /// <param name="parameters">Uses the <see cref="IList"/> as a source for the parameters.</param>
        /// <remarks>The index of the objects will be used as the order, from 0-n.</remarks>
        /// <exception cref="ArgumentException">Thrown when no parameters are provided.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when this method is called an no endpoint has been provided.</exception>
        IRequestContent WithParameters([NotNull] IList<object> parameters);
    }
}
