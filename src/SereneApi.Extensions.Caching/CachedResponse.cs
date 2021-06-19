using SereneApi.Abstractions.Content;
using System.Net;
using System.Net.Http;

namespace SereneApi.Extensions.Caching
{
    public class CachedResponse : ICachedResponse
    {
        public string Content { get; }

        public HttpStatusCode StatusCode { get; }

        public string ReasonPhrase { get; }

        private CachedResponse(HttpStatusCode statusCode, string content = null, string reasonPhrase = null)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Content = content;
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

        public static ICachedResponse FromHttpResponse(HttpResponseMessage responseMessage)
        {
            string content = new HttpContentResponse(responseMessage.Content).GetContentString();

            return new CachedResponse(responseMessage.StatusCode, content, responseMessage.ReasonPhrase);
        }
    }
}
