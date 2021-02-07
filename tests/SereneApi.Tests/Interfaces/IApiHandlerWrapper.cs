using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SereneApi.Tests.Interfaces
{
    public interface IApiHandlerWrapper: IDisposable
    {
        IConnectionConfiguration Connection { get; }

        IApiResponse PerformRequestWrapper(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null);

        IApiResponse<TResponse> PerformRequestWrapper<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null);

        Task<IApiResponse> PerformRequestAsyncWrapper(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null);

        Task<IApiResponse<TResponse>> PerformRequestAsyncWrapper<TResponse>(Method method, Expression<Func<IRequest, IRequestCreated>> factory = null);
    }
}
