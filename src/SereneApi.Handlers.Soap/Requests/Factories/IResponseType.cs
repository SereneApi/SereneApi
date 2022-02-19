namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IResponseType
    {
        IRequestPerformer<TResponse> RespondsWith<TResponse>() where TResponse : class;
    }
}