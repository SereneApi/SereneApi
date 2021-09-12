using SereneApi.Handlers.Rest.Tests.Mock;

namespace SereneApi.Handlers.Rest.Tests.Interfaces
{
    public interface ICrudApi : ICrudApi<MockPersonDto, long>
    {
    }
}