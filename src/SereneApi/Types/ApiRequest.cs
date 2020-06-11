using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SereneApi.Interfaces;

namespace SereneApi.Types
{
    public class ApiRequest : IApiRequest
    {
        public string EndPoint { get; }

        public Method Method { get; }

        public ApiRequest(Method method, string endPoint)
        {
            Method = method;
            EndPoint = endPoint;
        }
    }

}
