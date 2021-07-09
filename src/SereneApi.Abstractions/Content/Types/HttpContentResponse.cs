using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Content.Types
{
    public class HttpContentResponse : IResponseContent
    {
        private readonly HttpContent _content;

        public HttpContentResponse(HttpContent content)
        {
            _content = content;
        }

        public Stream GetContentStream()
        {
            return _content.ReadAsStreamAsync().Result;
        }

        public Task<Stream> GetContentStreamAsync()
        {
            return _content.ReadAsStreamAsync();
        }

        public string GetContentString()
        {
            return _content.ReadAsStringAsync().Result;
        }

        public Task<string> GetContentStringAsync()
        {
            return _content.ReadAsStringAsync();
        }
    }
}
