using SereneApi.Abstraction;
using SereneApi.Enums;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SereneApi.Tests
{
    public class TestApiHandler : ApiHandler
    {
        public TestApiHandler(IApiHandlerOptions options) : base(options)
        {
        }

        public new Task<IApiResponse> InPathRequestAsync(Method method, object endpoint = null)
        {
            return base.InPathRequestAsync(method, endpoint);
        }

        public new Task<IApiResponse> InPathRequestAsync(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            return base.InPathRequestAsync(method, endpointTemplate, endpointParameters);
        }

        public new Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, object endpoint = null)
        {
            return base.InPathRequestAsync<TResponse>(method, endpoint);
        }

        public new Task<IApiResponse<TResponse>> InPathRequestAsync<TResponse>(Method method, string endpointTemplate, params object[] endpointParameters)
        {
            return base.InPathRequestAsync<TResponse>(method, endpointTemplate, endpointParameters);
        }

        public new Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(Method method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, object endpoint = null) where TQueryContent : class
        {
            return base.InPathRequestWithQueryAsync<TResponse, TQueryContent>(method, queryContent, query, endpoint);
        }

        public new Task<IApiResponse<TResponse>> InPathRequestWithQueryAsync<TResponse, TQueryContent>(Method method, TQueryContent queryContent, Expression<Func<TQueryContent, object>> query, string endpointTemplate, params object[] endpointParameters) where TQueryContent : class
        {
            return base.InPathRequestWithQueryAsync<TResponse, TQueryContent>(method, queryContent, query, endpointTemplate, endpointParameters);
        }

        public new Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, object endpoint = null)
        {
            return base.InBodyRequestAsync<TContent>(method, inBodyContent, endpoint);
        }

        public new Task<IApiResponse> InBodyRequestAsync<TContent>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            return base.InBodyRequestAsync<TContent>(method, inBodyContent, endpointTemplate, endpointParameters);
        }

        public new Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, object endpoint = null)
        {
            return base.InBodyRequestAsync<TContent, TResponse>(method, inBodyContent, endpoint);
        }

        public new Task<IApiResponse<TResponse>> InBodyRequestAsync<TContent, TResponse>(Method method, TContent inBodyContent, string endpointTemplate, params object[] endpointParameters)
        {
            return base.InBodyRequestAsync<TContent, TResponse>(method, inBodyContent, endpointTemplate, endpointParameters);
        }
    }
}
