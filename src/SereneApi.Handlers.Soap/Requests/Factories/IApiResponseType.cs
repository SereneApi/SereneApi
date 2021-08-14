namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IApiResponseType
    {
        IApiRequestPerformer<TResponse> RespondsWith<TResponse>() where TResponse : class;
    }
}
