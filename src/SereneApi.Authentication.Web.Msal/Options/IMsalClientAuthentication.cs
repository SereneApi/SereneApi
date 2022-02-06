namespace SereneApi.Authentication.Web.Msal.Options
{
    public interface IMsalClientAuthentication : IMsalUserScopes
    {
        /// <summary>
        /// Enables the <see cref="MsalAuthenticator"/> to authenticate the application.
        /// </summary>
        IMsalUserScopes EnableClientAuthentication();
    }
}