namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// The options for a specific API.
    /// </summary>
    /// <typeparam name="TApi">The specific API these options are intended for.</typeparam>
    public interface IApiOptions<TApi>: IApiOptions where TApi : class
    {
    }
}
