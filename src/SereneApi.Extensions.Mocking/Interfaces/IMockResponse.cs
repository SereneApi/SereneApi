using SereneApi.Abstraction.Enums;
using SereneApi.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IMockResponse
    {
        ISerializer Serializer { get; }

        Status Status { get; }

        string Message { get; }

        Task<IApiRequestContent> GetResponseAsync(CancellationToken cancellationToken = default);
    }
}
