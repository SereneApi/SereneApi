using System;

namespace SereneApi.Core.Http.Responses.Exceptions
{
    public class UnsuccessfulResponseException : Exception
    {
        public IApiResponse Response { get; }

        public UnsuccessfulResponseException(IApiResponse response) : base($"Status: {response.Status} | Message: {response.Message}", response.Exception)
        {
            Response = response;
        }
    }
}