using SereneApi.Abstractions;
using SereneApi.Abstractions.Handler;
using SereneApi.Interfaces;
using SereneApi.Tests.Interfaces;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Abstractions.Responses;

namespace SereneApi.Tests.Mock
{
    public class ApiHandlerWrapper: ApiHandler, IApiHandlerWrapper
    {
        public ApiHandlerWrapper(IApiHandlerOptions options) : base(options)
        {
        }

        #region Sync Methods

        public new IApiResponse PerformRequest(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return base.PerformRequest(method, factory);
        }

        public new IApiResponse<TResponse> PerformRequest<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return base.PerformRequest<TResponse>(method, factory);
        }

        #endregion
        #region Async Methods

        public new Task<IApiResponse> PerformRequestAsync(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return base.PerformRequestAsync(method, factory);
        }

        public new Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null)
        {
            return base.PerformRequestAsync<TResponse>(method, factory);
        }

        #endregion
    }
}
