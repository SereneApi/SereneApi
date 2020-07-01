using SereneApi.Abstraction;
using SereneApi.Abstractions;
using SereneApi.Tests.Mock;

namespace SereneApi.Tests.Interfaces
{
    public interface ICrudApi: ICrudApi<MockPersonDto, long>
    {
    }
}
