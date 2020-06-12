using System;

namespace SereneApi.Interfaces
{
    public interface IRouteFactory
    {
        public string Resource { get; }

        public string ResourcePath { get; }

        void AddQuery(string queryString);

        void AddParameters(params object[] parameters);

        void AddEndpoint(string endpoint);

        Uri BuildRoute();
    }
}
