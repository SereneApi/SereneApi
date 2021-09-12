using SereneApi.Core.Content.Types;
using System.Net;
using System.Net.Http;

namespace SereneApi.Extensions.Caching
{
    public class CachedResponse : ICachedResponse
    {
        public string Content { get; }

        public string ReasonPhrase { get; }

        public HttpStatusCode StatusCode { get; }

        private CachedResponse(HttpStatusCode statusCode, string content = null, string reasonPhrase = null)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Content = content;
        }

        public static ICachedResponse FromHttpResponse(HttpResponseMessage responseMessage)
        {
            string content = new HttpContentResponse(responseMessage.Content).GetContentString();

            return new CachedResponse(responseMessage.StatusCode, content, responseMessage.ReasonPhrase);
        }

        public HttpResponseMessage GenerateHttpResponse()
        {
            HttpContent content = null;

            if (!string.IsNullOrWhiteSpace(Content))
            {
                content = (HttpContent)new JsonContent(Content).GetContent();
            }

            return new HttpResponseMessage
            {
                Content = content,
                StatusCode = StatusCode,
                ReasonPhrase = ReasonPhrase
            };
        }
    }
}