namespace SereneApi.Abstractions.Authentication
{
    public interface IAuthentication
    {
        string Scheme { get; }

        string Parameter { get; }
    }
}
