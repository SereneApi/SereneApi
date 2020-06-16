using SereneApi.Interfaces;
using System;
using System.Text;
using SereneApi.Interfaces.Requests;

namespace SereneApi.Types.Content
{
    public class JsonContent: IApiRequestContent
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
            if(!(obj is JsonContent content))
            {
                return false;
            }

            return Equals(content);
        }

        protected bool Equals(JsonContent other)
        {
            return Equals(Encoding, other.Encoding) && MediaType.Equals(other.MediaType) && Content == other.Content;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Encoding, MediaType, Content);
        }
    }
}
