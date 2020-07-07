using SereneApi.Abstractions.Request;

namespace SereneApi.Interfaces
{
    public interface IRequestCreated
    {
        /// <summary>
        /// Gets the request.
        /// </summary>
        IApiRequest GetRequest();
    }
}
