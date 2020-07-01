using SereneApi.Abstractions.Requests;

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
