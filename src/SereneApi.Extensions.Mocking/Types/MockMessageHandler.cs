using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Types.Content;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Types
{
    public class MockMessageHandler : HttpMessageHandler
    {
        private readonly IReadOnlyList<IMockResponse> _mockResponses;

        public MockMessageHandler(IReadOnlyList<IMockResponse> mockResponses)
        {
            _mockResponses = mockResponses;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            foreach (IMockResponse mockResponse in _mockResponses)
            {
                bool validatedResponse = true;

                if (mockResponse is IWhitelist whitelist)
                {
                    if (validatedResponse)
                    {
                        validatedResponse = whitelist.Validate(request.RequestUri);
                    }

                    if (validatedResponse)
                    {
                        validatedResponse = whitelist.Validate(request.Method);
                    }

                    if (request.Content != null)
                    {
                        string content = await request.Content.ReadAsStringAsync();

                        IApiRequestContent requestContent = new JsonContent(content);

                        if (validatedResponse)
                        {
                            whitelist.Validate(requestContent);
                        }

                    }
                }

                if (!validatedResponse)
                {
                    continue;
                }

                IApiRequestContent response = await mockResponse.GetResponseAsync(cancellationToken);

                if (response is JsonContent jsonContent)
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = mockResponse.Status.ToHttpStatusCode(),
                        Content = jsonContent.ToStringContent()
                    };
                }

                return new HttpResponseMessage
                {
                    StatusCode = mockResponse.Status.ToHttpStatusCode(),
                    ReasonPhrase = mockResponse.Message
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
