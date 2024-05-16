using SereneApi.Resource.Schema.Enums;

namespace SereneApi.Resource.Schema.Attributes.Parameter
{
    public sealed class HttpQueryAttribute : HttpParameterAttribute
    {
        public HttpQueryAttribute() : base(ApiRouteParameterType.Query)
        {
        }

        public HttpQueryAttribute(string name) : base(ApiRouteParameterType.Query, name)
        {
        }
    }
}
