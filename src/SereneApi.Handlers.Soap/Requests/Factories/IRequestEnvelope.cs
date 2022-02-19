namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IRequestEnvelope : IRequestService
    {
        IEnvelopeFactory<TEnvelope> UsingEnvelope<TEnvelope>();

        IResponseType UsingEnvelope<TEnvelope>(TEnvelope envelope);
    }
}