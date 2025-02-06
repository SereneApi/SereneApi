using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace SereneApi.Request
{
    public interface IApiRequest
    {
        HttpMethod Method { get; }

        string? Route { get; }

        string? Version { get; }

        string? Query { get; }

        string FullRoute { get; }

        object? Content { get; }

        Type? ResponseType { get; }

        CancellationToken CancellationToken { get; }

        IReadOnlyDictionary<string, string> Headers { get; }
    }
}
