using SereneApi.Resource.Schema.Enums;

namespace SereneApi.Resource.Schema.Attributes.Parameter
{
    public sealed class HttpQueryParameterAttribute : HttpParameterAttribute
    {
        public HttpQueryParameterAttribute() : base(ApiRouteParameterType.Query)
        {
        }

        public HttpQueryParameterAttribute(string name) : base(ApiRouteParameterType.Query, name)
        {
        }
    }
}
