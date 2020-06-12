using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Types
{
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _mockResponse;

        private Uri _requestUri;

        private int _timeoutsUntilSuccess;

        private uint _timeoutCount;

        private TimeSpan _waitTime = TimeSpan.Zero;

        private string _expectedContent;

        /// <summary>
        /// Will only ever return Timeouts.
        /// </summary>
        public MockHttpMessageHandler()
        {
            _timeoutsUntilSuccess = -1;
        }

        /// <summary>
        /// Returns a Mock Response.
        /// </summary>
        /// <param name="mockResponse">The response to be returned.</param>
        /// <param name="requestUri">The expected Request Uri, throws an exception if they do not add up.</param>
        public MockHttpMessageHandler(HttpResponseMessage mockResponse)
        {
            _mockResponse = mockResponse;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                string result = request.Content?.ReadAsStringAsync().Result;

                if (_waitTime != TimeSpan.Zero && _timeoutCount < _timeoutsUntilSuccess)
                {
                    Thread.Sleep(_waitTime);
                }

                if (_timeoutsUntilSuccess >= 0 || _timeoutCount == _timeoutsUntilSuccess)
                {
                    if (_requestUri == null || request.RequestUri == _requestUri)
                    {
                        return _mockResponse;
                    }

                    throw new ArgumentException($"Received an Incorrect Uri - Received:{request.RequestUri} | Expected:{_requestUri}");
                }

                _timeoutCount++;

                throw new TaskCanceledException();
            }, cancellationToken);
        }

        public void AddTimeoutUntilSuccess(int timeoutsUntilSuccess) => _timeoutsUntilSuccess = timeoutsUntilSuccess;

        public void AddWaitUntilSuccess(TimeSpan waitTime) => _waitTime = waitTime;

        public void CheckContent(string expectedContent) => _expectedContent = expectedContent;

        public void CheckRequestUri(Uri requestUri) => _requestUri = requestUri;
    }
}
