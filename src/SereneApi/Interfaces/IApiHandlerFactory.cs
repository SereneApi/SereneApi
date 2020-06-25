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
        TApiDefinition Build<TApiDefinition>() where TApiDefinition : class;
    }
}
