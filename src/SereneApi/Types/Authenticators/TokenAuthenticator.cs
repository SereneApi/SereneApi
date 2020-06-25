using SereneApi.Interfaces;
using System;
using System.Threading;

namespace SereneApi.Types.Authenticators
{
    public class TokenAuthenticator: IAuthenticator
    {
        private readonly TimerCallback _tokenRefresher;

        public IAuthentication GetAuthentication()
        {
            throw new NotImplementedException();
        }
    }
}
