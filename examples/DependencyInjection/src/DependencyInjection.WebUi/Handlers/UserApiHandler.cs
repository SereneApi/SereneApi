using DependencyInjection.API.Interfaces;
using SereneApi;
using SereneApi.DependencyInjection.Types;

namespace DependencyInjection.WebUi.Handlers
{
    public class UserApiHandler : ApiHandler, IUserApi
    {
        public UserApiHandler(ApiHandlerOptions<UserApiHandler> options) : base(options)
        {
        }
    }
}
