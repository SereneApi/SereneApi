namespace SereneApi.Authentication.WebAssembly.Msal.Options
{
    public interface IMsalAuthenticationOptionsBuilder
    {
        string ReturnUrl { get; set; }

        void RegisterScope(string scope);
    }
}