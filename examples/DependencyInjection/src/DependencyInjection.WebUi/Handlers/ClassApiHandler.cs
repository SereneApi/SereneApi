using DependencyInjection.API;
using DependencyInjection.API.DTOs;
using SereneApi.Core.Options;
using SereneApi.Handlers.Rest;

namespace DependencyInjection.WebUi.Handlers
{
    // ApiHandler is not used in this example and instead CrudApiHandler<ClassDto, long>.
    // ClassDto being the resource and long being the identity.
    // When this class is inherited it includes the base ApiHandler plus pre-implemented Crud methods.
    // This will become apparent in the Class Page.
    public class ClassApiHandler : CrudApiHandler<ClassDto, long>, IClassApi
    {
        public ClassApiHandler(IApiOptions<ClassApiHandler> options) : base(options)
        {
        }
    }
}
