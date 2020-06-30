using SereneApi.Interfaces;
using SereneApi.Interfaces.Requests;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SereneApi.Tests.Interfaces
{
    public interface IApiHandler: IDisposable
    {
        IConnectionSettings Connection { get; }

        IApiResponse PerformRequest(Method method, Expression<Func<IRequest, IRequestCreated>> request = null);

        IApiResponse<TResponse> PerformRequest<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> request = null);

        Task<IApiResponse> PerformRequestAsync(Method method, Expression<Func<IRequest, IRequestCreated>> request = null);

        Task<IApiResponse<TResponse>> PerformRequestAsync<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> request = null);
    }
}
