using SereneApi.Interfaces;

namespace SereneApi.Types
{
    public class RegisterApiHandlerExtensions : CoreOptions, IRegisterApiHandlerExtensions
    {
        public RegisterApiHandlerExtensions()
        {
        }

        public RegisterApiHandlerExtensions(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
        }
    }
}
