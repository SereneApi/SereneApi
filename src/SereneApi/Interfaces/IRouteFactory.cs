using System;

namespace SereneApi.Interfaces
{
    /// <summary>
    /// Builds a route using the supplied values.
    /// </summary>
    public interface IRouteFactory
    {
        /// <summary>
        /// The Resource Path appended to the route.
        /// </summary>
        public string ResourcePath { get; }

        /// <summary>
        /// Supplies the Resource to be used in the route.
        /// </summary>
        /// <param name="resource"></param>
        void AddResource(string resource);

        /// <summary>
        /// Supplies the Query to be appended to the end of the route.
        /// </summary>
        /// <param name="queryString"></param>
        void AddQuery(string queryString);

        /// <summary>
        /// Supplies the parameters to be appended to the route.
        /// A single parameter can be applied with ot without an End Point.
        /// If more than one parameter is provided a formattable End Point is required.
        /// </summary>
        /// <param name="parameters"></param>
        void AddParameters(params object[] parameters);

        /// <summary>
        /// Supplies the End Point to be appended to the route.
        /// The End Point can be submitted as a formattable string.
        /// If this is done parameters will be required.
        /// </summary>
        /// <param name="endPoint"></param>
        void AddEndPoint(string endPoint);

        /// <summary>
        /// The route will be built and returned as an <see cref="Uri"/>.
        /// Once the route has been built the settings will be cleared.
        /// </summary>
        Uri BuildRoute();

        /// <summary>
        /// Clears the provided values.
        /// </summary>
        void Clear();
    }
}
