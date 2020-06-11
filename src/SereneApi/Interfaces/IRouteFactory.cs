using System;

namespace SereneApi.Interfaces
{
    public interface IRouteFactory
    {
        void AddQuery(string queryString);

        void AddParameters(params object[] parameters);

        void AddEndpoint(string endpoint);

        Uri BuildRoute();
    }
}
