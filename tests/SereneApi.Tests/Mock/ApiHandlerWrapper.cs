using SereneApi.Interfaces.Requests;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SereneApi.Tests.Mock
{
    public class ApiHandlerWrapper: ApiHandler
    {
        public ApiHandlerWrapper(IApiHandlerOptions options) : base(options)
        {
        }

        #region Sync Methods

        public new IApiResponse PerformRequest(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            return base.PerformRequest(method, request);
        }

        public new IApiResponse<TResponse> PerformRequest<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            return base.PerformRequest<TResponse>(method, request);
        }

        #endregion
        #region Async Methods

        public new Task<IApiResponse> PerformRequestAsync(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            return base.PerformRequestAsync(method, request);
        }

        public new Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> request = null)
        {
            return base.PerformRequestAsync<TResponse>(method, request);
        }

        #endregion
    }
}
