using SereneApi.Handlers.Soap.Models;

namespace SereneApi.Handlers.Soap.Envelopment
{
    public interface IEnvelopeTranslator
    {
        T Unpack<T>(ISoapEnvelope envelope);
    }
}