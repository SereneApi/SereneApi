namespace SereneApi.Extensions.DependencyInjection.Interfaces
{
    /// <summary>
    /// The options used to create the <see cref="ApiHandler"/>.
    /// </summary>
    /// <remarks>This is required for <see cref="ApiHandler"/>s that will be instantiated with Dependency Injection.</remarks>
    /// <typeparam name="TApiHandler">The <see cref="ApiHandler"/> the options are intended for.</typeparam>
    public interface IApiHandlerOptions<TApiHandler>: IApiHandlerOptions where TApiHandler : class
    {
    }
}
