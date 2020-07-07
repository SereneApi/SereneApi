using System;

namespace SereneApi.Request
{
    public interface IRequestEndPoint: IRequestContent
    {
        /// <summary>
        /// Adds the parameter to the request endpoint.
        /// </summary>
        /// <param name="parameter">The parameter to be added to the endpoint.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        IRequestContent WithEndPoint(object parameter);
        /// <summary>
        /// Adds the parameter to the request endpoint.
        /// </summary>
        /// <param name="endPoint">The parameter to be added to the endpoint.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        IRequestContent WithEndPoint(string endPoint);

        /// <summary>
        /// Formats the template and adds it to the request endpoint.
        /// If template is not formattable one parameter can be provided.
        /// If a template is formattable multiple parameters can be supplied if the template support it.
        /// </summary>
        /// <param name="template">The template to be formatted.</param>
        /// <param name="parameters">The values to be appended to the template.</param>
        /// <exception cref="ArgumentNullException">Thrown if null values are provided.</exception>
        /// <exception cref="FormatException">Thrown if supplied template is invalid.</exception>
        IRequestContent WithEndPointTemplate(string template, params object[] parameters);
    }
}
