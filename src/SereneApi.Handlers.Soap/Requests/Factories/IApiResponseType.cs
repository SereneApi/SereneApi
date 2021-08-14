namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IApiResponseType
    {
        IApiRequestPerformer<TResponds> RespondsWith<TResponds>() where TResponds : class;
    }
}
