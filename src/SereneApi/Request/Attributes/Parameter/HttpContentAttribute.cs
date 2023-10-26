using SereneApi.Resource.Schema.Enums;

namespace SereneApi.Request.Attributes.Parameter
{
    public sealed class HttpContentAttribute : HttpParameterAttribute
    {
        public HttpContentAttribute() : base(ApiEndpointParameterType.Content)
        {
        }
    }
}
