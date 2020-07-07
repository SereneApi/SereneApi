namespace SereneApi.Abstractions.Authentication
{
    public class TokenInfo
    {
        public string Token { get; }

        public int ExpiryTime { get; }

        public TokenInfo(string token, int expiryTime = 0)
        {
            Token = token;
            ExpiryTime = expiryTime;
        }
    }
}
