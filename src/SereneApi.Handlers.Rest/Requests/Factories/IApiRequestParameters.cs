using System;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestParameters : IApiRequestQuery
    {
        /// <summary>
        /// Specifies the parameter to be used in the request.
        /// </summary>
        /// <param name="parameter">The parameter to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <remarks>
        /// The parameter is converted into a <see cref="string"/> using the ToString() method.
        /// </remarks>
        IApiRequestQuery WithParameter(object parameter);

        /// <summary>
        /// Specifies the parameters to be used in the request.
        /// </summary>
        /// <param name="parameters">The parameters to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when an empty collection is provided.</exception>
        /// <remarks>
        /// All parameters are converted into <see cref="string"/> s using the ToString() method.
        /// </remarks>
        IApiRequestQuery WithParameters(params object[] parameters);
    }
}