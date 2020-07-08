namespace SereneApi.Abstractions.Queries
{
    public interface IQueryConverter<in T>: IQueryConverter
    {
        string Convert(T value);
    }
}
