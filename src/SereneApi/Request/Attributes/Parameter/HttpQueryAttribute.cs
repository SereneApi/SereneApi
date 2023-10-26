using SereneApi.Resource.Schema.Enums;

namespace SereneApi.Request.Attributes.Parameter
{
    public sealed class HttpQueryAttribute : HttpParameterAttribute
    {
        public HttpQueryAttribute() : base(ApiEndpointParameterType.Query)
        {
        }

        public HttpQueryAttribute(string name) : base(ApiEndpointParameterType.Query, name)
        {
        }
    }
}
