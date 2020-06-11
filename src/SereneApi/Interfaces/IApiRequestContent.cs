using System.Text;
using SereneApi.Types;

namespace SereneApi.Interfaces
{
    public interface IApiRequestContent
    {
        Encoding Encoding { get; }

        MediaType MediaType { get; }
    }
}
