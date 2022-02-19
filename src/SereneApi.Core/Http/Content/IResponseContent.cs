using System.IO;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Content
{
    public interface IResponseContent
    {
        Stream GetContentStream();

        Task<Stream> GetContentStreamAsync();

        string GetContentString();

        Task<string> GetContentStringAsync();
    }
}