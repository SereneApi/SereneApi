namespace SereneApi.Interfaces
{
    /// <summary>
    /// Builds instances of the request <see cref="ApiHandler"/>.
    /// </summary>
    public interface IApiHandlerFactory
    {
        /// <summary>
        /// Creates a new instance of the requested <see cref="ApiHandler"/> type.
        /// </summary>
        /// <typeparam name="TApiHandler">The type to be instantiated that inherits from <see cref="TApiHandler"/>.</typeparam>
        TApiHandler Build<TApiHandler>() where TApiHandler : ApiHandler;
    }
}
