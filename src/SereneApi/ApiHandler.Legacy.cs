using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SereneApi
{
    /// <summary>
    /// Contains Legacy Methods InPathRequest and InBodyRequest from the Original <see cref="ApiHandler"/>.
    /// This will be removed in an upcoming release. This code is no longer being tested.
    /// </summary>
    public abstract partial class ApiHandler
    {
        /// <summary>
        /// Performs an in Path Request. The <see cref="endpoint"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the Url</param>
        [Obsolete("This method has been superseded by PerformRequestAsync")]
        protected virtual Task<IApiResponse> InPathRequestAsync(Method method, object endpoint = null)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpoint));
        }

        /// <summary>
        /// Performs an in Path Request. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        [Obsolete("This method has been superseded by PerformRequestAsync")]
        protected virtual Task<IApiResponse> InPathRequestAsync(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters));
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpoint"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the Url</param>
        [Obsolete("This method has been superseded by PerformRequestAsync<TResponse>")]
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, object endpoint = null)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpoint));
        }

        /// <summary>
        /// Performs an in Path Request returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        [Obsolete("This method has been superseded by PerformRequestAsync<TResponse>")]
        protected virtual Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters));
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQuery">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be performed</param>
        /// <param name="queryContent"> <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        [Obsolete("This method has been superseded by PerformRequestAsync<TResponse>")]
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQuery>(Method method, TQuery queryContent, Expression<Func<TQuery, object>> query, object endpoint = null) where TQuery : class
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpoint)
                .WithQuery(queryContent, query));
        }

        /// <summary>
        /// Performs an in Path Request with query support returning a <see cref="TResponse"/>. The <see cref="endpointParameters"/> will be appended to the Url
        /// </summary>
        /// <typeparam name="TResponse">The type to be deserialized by the <see cref="ApiHandler"/> from the response</typeparam>
        /// <typeparam name="TQuery">The type to be sent in the query</typeparam>
        /// <param name="method">The RESTful API <see cref="Method"/> to be used</param>
        /// <param name="endpointTemplate">The endpoint to be performed, supports templates for string formatting with <see cref="endpointParameters"/></param>
        /// <param name="queryContent">The <see cref="queryContent"/> to be used when generating the <see cref="query"/></param>
        /// <param name="query">Selects parts of the <see cref="queryContent"/> to be converted into a query</param>
        /// <param name="endpointParameters">The <see cref="endpointParameters"/> to be appended to the Url</param>
        [Obsolete("This method has been superseded by PerformRequestAsync<TResponse>")]
        protected virtual Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQuery>(Method method, TQuery queryContent, Expression<Func<TQuery, object>> query, string endpointTemplate, params object[] endpointParameters) where TQuery : class
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters)
                .WithQuery(queryContent, query));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent">The type to be serialized and sent in the body of the request</typeparam>
        /// <param name="method">The RESTful <see cref="Method"/> to be used</param>
        /// <param name="inBodyContent">The object serialized and sent in the body of the request</param>
        /// <param name="endpoint">The <see cref="endpoint"/> to be appended to the end of the Url</param>
        [Obsolete("This method has been superseded by PerformRequestAsync")]
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, object endpoint = null)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpoint)
                .WithInBodyContent(inBodyContent));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="method"></param>
        /// <param name="inBodyContent"></param>
        /// <param name="endpointTemplate"></param>
        /// <param name="endpointParameters"></param>
        [Obsolete("This method has been superseded by PerformRequestAsync")]
        protected virtual Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters)
                .WithInBodyContent(inBodyContent));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="inBodyContent"></param>
        /// <param name="endpoint"></param>
        [Obsolete("This method has been superseded by PerformRequestAsync<TResponse>")]
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, object endpoint = null)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpoint)
                .WithInBodyContent(inBodyContent));
        }

        /// <summary>
        /// Serializes the supplied <typeparam name="TContent"/> sending it in the Body of the Request
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="inBodyContent"></param>
        /// <param name="endpointTemplate"></param>
        /// <param name="endpointParameters"></param>
        [Obsolete("This method has been superseded by PerformRequestAsync<TResponse>")]
        protected virtual Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            return PerformRequestAsync<TResponse>(method, request => request
                .WithEndPoint(endpointTemplate, endpointParameters)
                .WithInBodyContent(inBodyContent));
        }
    }
}
