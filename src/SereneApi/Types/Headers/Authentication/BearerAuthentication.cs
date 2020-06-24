using SereneApi.Interfaces;

namespace SereneApi.Types.Headers.Authentication
{
    public readonly class BearerAuthentication: IAuthentication
    {
        public string Scheme => "Bearer";

        public string Parameter { get; }

        public BearerAuthentication(string token)
        {
            Parameter = token;
        }
    }
}
