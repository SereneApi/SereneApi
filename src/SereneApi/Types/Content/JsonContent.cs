using SereneApi.Interfaces;
using System.Text;

namespace SereneApi.Types.Content
{
    public class JsonContent : IApiRequestContent
    {
        public Encoding Encoding { get; }

        public MediaType MediaType { get; }

        public string Content { get; }

        public JsonContent(string content)
        {
            Content = content;
            Encoding = Encoding.UTF8;
            MediaType = MediaType.ApplicationJson;
        }

        public JsonContent(string content, Encoding encoding, MediaType mediaTypeType)
        {
            Content = content;
            Encoding = encoding;
            MediaType = mediaTypeType;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is JsonContent content))
            {
                return false;
            }

            bool equals = content.Content == Content;

            equals = content.MediaType == MediaType;
            equals = content.Encoding == Encoding;

            return equals;
        }
    }
}
