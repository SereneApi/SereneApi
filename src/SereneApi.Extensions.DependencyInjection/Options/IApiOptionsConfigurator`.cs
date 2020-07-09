using Microsoft.Extensions.Configuration;
using SereneApi.Abstractions.Options;

namespace SereneApi.Extensions.DependencyInjection.Options
{
    public interface IApiOptionsConfigurator<TApiDefinition>: IApiOptionsConfigurator where TApiDefinition : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        void UseConfiguration(IConfiguration configuration);
    }
}
