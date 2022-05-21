using System.Collections.Generic;

namespace SereneApi.Handlers.Rest.Queries
{
    public interface IQuerySerializer
    {
        string Serialize(Dictionary<string, string> query);
    }
}