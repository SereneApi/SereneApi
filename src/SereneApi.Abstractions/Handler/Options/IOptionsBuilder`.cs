namespace SereneApi.Abstractions.Handler.Options
{
    public interface IOptionsBuilder<TApiDefinition>: IOptionsBuilder where TApiDefinition : class
    {
        new IOptions<TApiDefinition> BuildOptions();
    }
}
