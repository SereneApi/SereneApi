namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// The options used to create the <see cref="ApiHandler"/>.
    /// </summary>
    /// <remarks>This is required for <see cref="ApiHandler"/>s that will be instantiated with Dependency Injection.</remarks>
    public interface IApiOptions<TApi>: IApiOptions where TApi : class
    {
    }
}
