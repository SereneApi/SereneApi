using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking.Enums;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Types.Content;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Dictionary<int, IMockResponse> weightedResponses = new Dictionary<int, IMockResponse>();

            foreach (IMockResponse mockResponse in _mockResponses)
            {
                int responseWeight = await GetResponseWeightAsync(mockResponse, request);

                // If two responses have the same weight the older response will be ignored.
                if (responseWeight >= 0 && !weightedResponses.ContainsKey(responseWeight))
                {
                    weightedResponses.Add(responseWeight, mockResponse);
                }
            }

            if (weightedResponses.Count <= 0)
            {
                throw new ArgumentException("No response was found.");
            }

            int maxWeight = weightedResponses.Keys.Max();

            IMockResponse response = weightedResponses[maxWeight];

            return await GetResponseMessageAsync(response, cancellationToken);
        }

        /// <summary>
        /// Override this method if you have added more <see cref="IWhitelist"/> dependencies for Responses.
        /// </summary>
        protected async Task<int> GetResponseWeightAsync(IMockResponse mockResponse, HttpRequestMessage request)
        {
            int responseWeight = 0;

            Validity validity = mockResponse.Validate(request.RequestUri);

            switch (validity)
            {
                case Validity.NotApplicable:
                    break;
                case Validity.Valid:
                    responseWeight++;
                    break;
                case Validity.Invalid:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            validity = mockResponse.Validate(request.Method.ToMethod());

            switch (validity)
            {
                case Validity.NotApplicable:
                    break;
                case Validity.Valid:
                    responseWeight++;
                    break;
                case Validity.Invalid:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (request.Content == null)
            {
                return responseWeight;
            }

            string content = await request.Content.ReadAsStringAsync();

            IApiRequestContent requestContent = new JsonContent(content);

            validity = mockResponse.Validate(requestContent);

            switch (validity)
            {
                case Validity.NotApplicable:
                    break;
                case Validity.Valid:
                    responseWeight++;
                    break;
                case Validity.Invalid:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return responseWeight;
        }

        protected async Task<HttpResponseMessage> GetResponseMessageAsync(IMockResponse mockResponse, CancellationToken cancellationToken)
        {
            IApiRequestContent requestContent = await mockResponse.GetResponseAsync(cancellationToken);

            if (requestContent is JsonContent jsonContent)
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
    }
}
