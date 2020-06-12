using SereneApi.Interfaces;
using System.Text;

namespace SereneApi.Types.Content
{
    public class JsonContent : IApiRequestContent
    {
        public Encoding Encoding { get; }

        public MediaType MediaType { get; }

        public string Content { get; }

        public JsonContent(string content, Encoding encoding, MediaType mediaTypeType)
        {
            Content = content;
            Encoding = encoding;
            MediaType = mediaTypeType;
        }
    }
}
