using SereneApi.Core.Serialization;

namespace SereneApi.Handlers.Soap.Serialization
{
    public interface ISoapSerializer : ISerializer
    {
        string SerializeToString(object value);
    }
}