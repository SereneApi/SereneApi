namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsBuilder<TApi>: IApiOptionsBuilder where TApi : class
    {
        new IApiOptions<TApi> BuildOptions();
    }
}
