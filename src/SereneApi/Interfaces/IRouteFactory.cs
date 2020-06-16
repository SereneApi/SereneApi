using System;

namespace SereneApi.Interfaces
{
    public interface IRouteFactory
    {
        public string ResourcePath { get; }

        void WithResource(string resource);

        void AddQuery(string queryString);

        void AddParameters(params object[] parameters);

        void AddEndPoint(string endPoint);

        /// <summary>
        /// Builds the route, if the resource has not been supplied a resource can be supplied here.
        /// If a resource has been set an exception will be thrown.
        /// </summary>
        Uri BuildRoute();
    }
}
