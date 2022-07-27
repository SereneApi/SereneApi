using SereneApi.Handlers.Rest.Configuration.Handler;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Configuration.Handler
{
    public static class HandlerConfigurationExtensions
    {
        public static string GetRouteTemplate(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<string>(RestHandlerConfigurationKeys.RouteTemplate);
        }

        public static void SetRouteTemplate(this HandlerConfiguration handlerConfiguration, string routeTemplate)
        {
            handlerConfiguration.Add(RestHandlerConfigurationKeys.RouteTemplate, routeTemplate);
        }
    }
}