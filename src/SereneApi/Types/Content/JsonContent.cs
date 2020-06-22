using SereneApi.Interfaces.Requests;
using System;
using System.Net.Http;
using System.Text;

namespace SereneApi.Types.Content
{
    public readonly struct JsonContent: IApiRequestContent
    {
        /// <summary>
        /// The <see cref="Encoding"/> of the in body content.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// The <see cref="MediaType"/> of the in body content.
        /// </summary>
        public MediaType MediaType { get; }

        public string Content { get; }

        public JsonContent(string content)
        {
            Content = content;
            Encoding = Encoding.UTF8;
            MediaType = MediaType.Json;
        }

        public JsonContent(string content, Encoding encoding, MediaType mediaTypeType)
        {
            Content = content;
            Encoding = encoding;
            MediaType = mediaTypeType;
        }

        public object GetContent()
        {
            return new StringContent(Content, Encoding, MediaType.TypeString);
        }

        public override bool Equals(object obj)
        {
            if(!(obj is JsonContent content))
            {
                return false;
            }

            return Equals(Encoding, content.Encoding) && MediaType.Equals(content.MediaType) && Content == content.Content;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Encoding, MediaType, Content);
        }
    }
}
