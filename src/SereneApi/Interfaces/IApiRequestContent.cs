using SereneApi.Types;
using System.Text;

namespace SereneApi.Interfaces
{
    public interface IApiRequestContent
    {
        Encoding Encoding { get; }

        MediaType MediaType { get; }
    }
}
