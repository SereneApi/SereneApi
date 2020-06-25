using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using SereneApi.Interfaces;

namespace SereneApi.Types.Authenticators
{
    public class TokenAuthenticator : IAuthenticator
    {
        private TimerCallback _tokenRefresher;

        public IAuthentication GetAuthentication()
        {
            throw new NotImplementedException();
        }
    }
}
