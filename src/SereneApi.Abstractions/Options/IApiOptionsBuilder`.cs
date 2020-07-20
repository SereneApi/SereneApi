namespace SereneApi.Abstractions.Options
{
    /// <summary>
    /// Builds API options using the specific configuration.
    /// </summary>
    /// <typeparam name="TApi">The specific API these options are intended for.</typeparam>
    public interface IApiOptionsBuilder<TApi>: IApiOptionsBuilder where TApi : class
    {
        /// <summary>
        /// Builds options for an API.
        /// </summary>
        new IApiOptions<TApi> BuildOptions();
    }
}
