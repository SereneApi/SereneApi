using System;

namespace SereneApi.Interfaces
{
    public interface IApiRequest
    {
        Uri EndPoint { get; }

        Method Method { get; }

        IApiRequestContent Content { get; }
    }
}
