using SereneApi.Core.Content;
using SereneApi.Core.Content.Types;
using SereneApi.Extensions.Mocking.Dependencies.Whitelist;
using SereneApi.Extensions.Mocking.Extensions;
using SereneApi.Extensions.Mocking.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking
{
    /// <summary>
    /// Handles processing of <see cref="IMockResponse"/>.
    /// </summary>
    /// <remarks>Override this class if you wish to extend or change its behaviour.</remarks>
    public class MockMessageHandler : DelegatingHandler
    {
        private readonly IReadOnlyList<IMockResponse> _mockResponses;

        /// <summary>
        /// Created a new instance of <see cref="MockMessageHandler"/>.
        /// </summary>
        /// <param name="mockResponses">The mock responses the <see cref="MockMessageHandler"/> will respond with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        public MockMessageHandler(IReadOnlyList<IMockResponse> mockResponses)
        {
            if (mockResponses == null)
            {
                throw new ArgumentNullException(nameof(mockResponses));
            }

            if (mockResponses.Count <= 0)
            {
                throw new ArgumentException($"{nameof(mockResponses)} must not be empty.");
            }

            _mockResponses = mockResponses;
        }

        /// <summary>
        /// Created a new instance of <see cref="MockMessageHandler"/>.
        /// </summary>
        /// <param name="mockResponses">The mock responses the <see cref="MockMessageHandler"/> will respond with.</param>
        /// <param name="messageHandler">Will process outgoing requests if no <see cref="IMockResponse"/> is available.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        /// <remarks>When a client handler is supplied it will allow outgoing requests if no mock response is found.</remarks>
        public MockMessageHandler(IReadOnlyList<IMockResponse> mockResponses, [NotNull] HttpMessageHandler messageHandler) : this(mockResponses)
        {
            InnerHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        /// <exception cref="NullReferenceException">Thrown if there is no <see cref="IMockResponse"/> for the request and no <see cref="HttpClientHandler"/> was provided.</exception>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

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
                if (InnerHandler == null)
                {
                    // No client was provided, so an error is thrown as no response was found.
                    throw new NullReferenceException($"No response was found for the {request.Method.ToMethod().ToString().ToUpper()} request to {request.RequestUri}");
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
        /// Gets the weight of the <see cref="IMockResponse"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        /// <remarks>Override this method if you have added custom <see cref="IWhitelist"/>.</remarks>
        protected virtual async Task<int> GetResponseWeightAsync([NotNull] IMockResponse mockResponse, HttpRequestMessage request)
        {
            if (mockResponse == null)
            {
                throw new ArgumentNullException(nameof(mockResponse));
            }

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

            IRequestContent requestContent = new JsonContent(content);

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

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <param name="mockResponse">The response to be returned.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is supplied.</exception>
        protected virtual async Task<HttpResponseMessage> GetResponseMessageAsync([NotNull] IMockResponse mockResponse, CancellationToken cancellationToken)
        {
            if (mockResponse == null)
            {
                throw new ArgumentNullException(nameof(mockResponse));
            }

            IRequestContent requestContent = await mockResponse.GetResponseContentAsync(cancellationToken);

            if (requestContent != null)
            {
                return new HttpResponseMessage
                {
                    StatusCode = mockResponse.Status.ToHttpStatusCode(),
                    Content = (HttpContent)requestContent.GetContent()
                };
            }

            return new HttpResponseMessage
            {
                StatusCode = mockResponse.Status.ToHttpStatusCode()
            };
        }

        #region IDisposeble

        private volatile bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if (disposing)
            {
                foreach (IMockResponse mockResponse in _mockResponses)
                {
                    mockResponse.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
