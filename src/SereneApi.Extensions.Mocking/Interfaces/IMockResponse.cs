using SereneApi.Interfaces;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IMockResponse
    {
        ISerializer Serializer { get; }

        Task<IApiRequestContent> GetResponseAsync();
    }
}
