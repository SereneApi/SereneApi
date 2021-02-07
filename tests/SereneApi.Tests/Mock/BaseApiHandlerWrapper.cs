using SereneApi.Abstractions.Options;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using SereneApi.Tests.Interfaces;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SereneApi.Tests.Mock
{
    public class BaseApiHandlerWrapper: BaseApiHandler, IApiHandlerWrapper
    {
        public BaseApiHandlerWrapper(IApiOptions options) : base(options)
        {
        }

        #region Sync Methods

        public IApiResponse PerformRequestWrapper(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return PerformRequest(method, factory);
        }

        public IApiResponse<TResponse> PerformRequestWrapper<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return PerformRequest<TResponse>(method, factory);
        }

        #endregion
        #region Async Methods

        public Task<IApiResponse> PerformRequestAsyncWrapper(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return PerformRequestAsync(method, factory);
        }

        public Task<IApiResponse<TResponse>> PerformRequestAsyncWrapper<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return PerformRequestAsync<TResponse>(method, factory);
        }

        #endregion
    }
}
