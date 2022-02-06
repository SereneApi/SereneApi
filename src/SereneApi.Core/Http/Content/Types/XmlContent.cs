using SereneApi.Core.Http.Media;
using System;
using System.Net.Http;
using System.Text;

namespace SereneApi.Core.Http.Content.Types
{
    public class XmlContent : IRequestContent
    {
        public string Content { get; }

        /// <summary>
        /// The <see cref="Encoding"/> of the in body content.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// The <see cref="MediaType"/> of the in body content.
        /// </summary>
        public MediaType MediaType { get; }

        public XmlContent(string content)
        {
            Content = content;
            Encoding = Encoding.UTF8;
            MediaType = MediaType.Xml;
        }

        public override bool Equals(object obj)
        {
            if (obj is not XmlContent content)
            {
                return false;
            }

            return Equals(Encoding, content.Encoding) && MediaType.Equals(content.MediaType) && Content == content.Content;
        }

        public object GetContent()
        {
            return new StringContent(Content, Encoding, MediaType.GetTypeString());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Encoding, MediaType, Content);
        }
    }
}