using SereneApi.Extensions.Mocking.Interfaces;

namespace SereneApi.Extensions.Mocking.Types.Dependencies
{
    public class MethodWhitelistDependency : IWhitelist
    {
        private readonly Method _method;

        public MethodWhitelistDependency(Method method)
        {
            _method = method;
        }

        public bool Validate(object value)
        {
            bool validated = true;

            if (value is Method method)
            {
                validated = method == _method;
            }

            return validated;
        }
    }
}
