using SereneApi.Interfaces;

namespace SereneApi.Types
{
    public class ApiHandlerExtensions : CoreOptions, IApiHandlerExtensions
    {
        public ApiHandlerExtensions()
        {
        }

        public ApiHandlerExtensions(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
        }
    }
}
