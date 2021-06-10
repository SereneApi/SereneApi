namespace SereneApi.Requests
{
    public interface IApiRequestResponseContent : IApiRequestPerformer
    {
        IApiRequestPerformer<TContent> RespondsWithContent<TContent>();
    }
}
