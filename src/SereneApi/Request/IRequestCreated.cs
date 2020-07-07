using SereneApi.Abstractions.Request;

namespace SereneApi.Request
{
    public interface IRequestCreated
    {
        /// <summary>
        /// Gets the request.
        /// </summary>
        IApiRequest GetRequest();
    }
}
