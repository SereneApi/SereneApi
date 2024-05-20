using SereneApi.Request;
using System;
using System.Net;

namespace SereneApi.Response
{
    public interface IApiResponse
    {
        TimeSpan Duration { get; }

        IApiRequest Request { get; }

        HttpStatusCode StatusCode { get; }
    }
}
