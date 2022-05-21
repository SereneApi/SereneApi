using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Handlers.Rest.Queries;
using SereneApi.Handlers.Rest.Tests.Mock;

namespace SereneApi.Handlers.Rest.Tests.Factories
{
    public class QueryFactoryShould
    {
        private readonly IQuerySerializer _querySerializer;

        public QueryFactoryShould()
        {
            ApiConfigurationManager configuration = new ApiConfigurationManager();

            configuration.AddApiConfiguration<BaseApiHandlerWrapper>(c =>
            {
                c.SetSource("http://localhost");
            });

            IApiSettings settings = configuration.BuildApiSettings<BaseApiHandlerWrapper>();

            _querySerializer = settings.Dependencies.GetRequiredDependency<IQuerySerializer>();
        }
    }
}