namespace SereneApi.Abstractions.Authentication
{
    public class BearerAuthentication: IAuthentication
    {
        public string Scheme => "Bearer";

        public string Parameter { get; }

        public BearerAuthentication(string token)
        {
            Parameter = token;
        }
    }
}
