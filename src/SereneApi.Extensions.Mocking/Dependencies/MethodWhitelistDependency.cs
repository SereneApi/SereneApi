using SereneApi.Abstractions.Request;

namespace SereneApi.Extensions.Mocking.Dependencies
{
    public class MethodWhitelistDependency: IWhitelist
    {
        private readonly Method _method;

        public MethodWhitelistDependency(Method method)
        {
            _method = method;
        }

        public Validity Validate(object value)
        {
            if(!(value is Method method))
            {
                return Validity.NotApplicable;
            }

            if(method == _method)
            {
                return Validity.Valid;
            }

            return Validity.Invalid;
        }
    }
}
