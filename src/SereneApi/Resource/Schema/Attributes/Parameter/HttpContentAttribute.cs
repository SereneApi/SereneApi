using SereneApi.Resource.Schema.Enums;

namespace SereneApi.Resource.Schema.Attributes.Parameter
{
    public sealed class HttpContentAttribute : HttpParameterAttribute
    {
        public HttpContentAttribute() : base(ApiRouteParameterType.Content)
        {
        }
    }
}
