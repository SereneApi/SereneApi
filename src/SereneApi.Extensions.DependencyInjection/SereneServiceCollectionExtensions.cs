using Microsoft.Extensions.DependencyInjection;

namespace SereneApi.Extensions.DependencyInjection
{
    public static class SereneServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApi<TApi>(this IServiceCollection services)
        {
            return services;
        }
    }
}
