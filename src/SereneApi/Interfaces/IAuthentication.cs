namespace SereneApi.Interfaces
{
    public interface IAuthentication
    {
        string Scheme { get; }

        string Parameter { get; }
    }
}
