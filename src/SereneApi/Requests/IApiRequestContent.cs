namespace SereneApi.Requests
{
    public interface IApiRequestContent : IApiRequestResponseContent
    {
        IApiRequestResponseContent AddInBodyContent<TContent>(TContent content);
    }
}
