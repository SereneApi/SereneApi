using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking.Enums;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Interfaces.Requests;
using SereneApi.Types.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Types
{
    /// <summary>
    /// The <see cref="HttpMessageHandler"/> used by the <see cref="ApiHandler"/> when running in Mock Mode.
    /// </summary>
    /// <remarks>Override this class if you wish to extend or change its behaviour.</remarks>
    public class MockMessageHandler: DelegatingHandler
    {
        private readonly IReadOnlyList<IMockResponse> _mockResponses;

        /// <summary>
        /// Created a new instance of the <see cref="MockMessageHandler"/>.
        /// </summary>
        /// <param name="mockResponsesBuilder">The builder which will be used to build the <see cref="IMockResponse"/>s.</param>
        /// <exception cref="ArgumentException">Thrown if a response is not found for the correct request.</exception>
        public MockMessageHandler(IMockResponsesBuilder mockResponsesBuilder)
        {
            if(mockResponsesBuilder is MockResponsesBuilder builder)
            {
                _mockResponses = builder.Build();
            }
        }

        /// <summary>
        /// Created a new instance of the <see cref="MockMessageHandler"/>.
        /// </summary>
        /// <param name="clientHandler">Will process outgoing requests if no <see cref="IMockResponse"/> is available.</param>
        /// <param name="mockResponsesBuilder">The builder which will be used to build the <see cref="IMockResponse"/>s.</param>
        public MockMessageHandler(HttpClientHandler clientHandler, IMockResponsesBuilder mockResponsesBuilder) : this(mockResponsesBuilder)
        {
            InnerHandler = clientHandler;
        }

        /// <exception cref="ArgumentException">Thrown if there is no <see cref="IMockResponse"/> for the request.</exception>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Dictionary<int, IMockResponse> weightedResponses = new Dictionary<int, IMockResponse>();

            foreach(IMockResponse mockResponse in _mockResponses)
            {
                int responseWeight = await GetResponseWeightAsync(mockResponse, request);

                // If two responses have the same weight the older response will be ignored.
                if(responseWeight >= 0 && !weightedResponses.ContainsKey(responseWeight))
                {
                    weightedResponses.Add(responseWeight, mockResponse);
                }
            }

            if(weightedResponses.Count <= 0)
            {
                if(InnerHandler == null)
                {
                    // No client was provided, so an error is thrown as no response was found.
                    throw new ArgumentException($"No response was found for the {request.Method.ToMethod().ToString().ToUpper()} request to {request.RequestUri}");
                }

                return await base.SendAsync(request, cancellationToken);

                // Since a client was provided, it will perform a normal request.
                //return await _internalClient.SendAsync(request, cancellationToken);
            }

            int maxWeight = weightedResponses.Keys.Max();

            IMockResponse response = weightedResponses[maxWeight];

            return await GetResponseMessageAsync(response, cancellationToken);
        }

        /// <summary>
        /// Override this method if you have added more <see cref="IWhitelist"/> dependencies for Responses.
        /// </summary>
        protected virtual async Task<int> GetResponseWeightAsync(IMockResponse mockResponse, HttpRequestMessage request)
        {
            int responseWeight = 0;

            Validity validity = mockResponse.Validate(request.RequestUri);

            switch(validity)
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

            switch(validity)
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

            if(request.Content == null)
            {
                return responseWeight;
            }

            string content = await request.Content.ReadAsStringAsync();

            IApiRequestContent requestContent = new JsonContent(content);

            validity = mockResponse.Validate(requestContent);

            switch(validity)
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

        protected virtual async Task<HttpResponseMessage> GetResponseMessageAsync(IMockResponse mockResponse, CancellationToken cancellationToken)
        {
            IApiRequestContent requestContent = await mockResponse.GetResponseContentAsync(cancellationToken);

            if(requestContent != null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = mockResponse.Status.ToHttpStatusCode(),
                    Content = (HttpContent)requestContent.GetContent()
                };
            }

            return new HttpResponseMessage
            {
                StatusCode = mockResponse.Status.ToHttpStatusCode(),
                ReasonPhrase = mockResponse.Message
            };
        }

        #region IDisposeble

        private volatile bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if(_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if(disposing)
            {
                foreach(IMockResponse mockResponse in _mockResponses)
                {
                    if(mockResponse is IDisposable disposableResponse)
                    {
                        disposableResponse.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
