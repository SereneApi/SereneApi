using SereneApi.Resource.Schema.Enums;

namespace SereneApi.Resource.Schema.Attributes.Parameter
{
    public sealed class HttpContentParameterAttribute : HttpParameterAttribute
    {
        public HttpContentParameterAttribute() : base(ApiRouteParameterType.Content)
        {
        }
    }
}
