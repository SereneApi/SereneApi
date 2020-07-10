namespace SereneApi.Abstractions.Request
{
    public interface IRequestCreated
    {
        /// <summary>
        /// Gets the request.
        /// </summary>
        IApiRequest GetRequest();
    }
}
