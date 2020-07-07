namespace SereneApi.Abstractions.Options
{
    public interface IApiOptionsBuilder<TApiDefinition>: IApiOptionsBuilder where TApiDefinition : class
    {
        new IApiOptions<TApiDefinition> BuildOptions();
    }
}
