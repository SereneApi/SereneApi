using System.Text;
using SereneApi.Types;

namespace SereneApi.Interfaces.Requests
{
    /// <summary>
    /// The content to be sent in the body of an <see cref="IApiRequest"/>.
    /// </summary>
    public interface IApiRequestContent
    {
        /// <summary>
        /// The <see cref="Encoding"/> of the in body content.
        /// </summary>
        Encoding Encoding { get; }
        
        /// <summary>
        /// The <see cref="MediaType"/> of the in body content.
        /// </summary>
        MediaType MediaType { get; }
    }
}
