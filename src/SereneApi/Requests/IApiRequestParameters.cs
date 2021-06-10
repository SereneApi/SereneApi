namespace SereneApi.Requests
{
    public interface IApiRequestParameters : IApiRequestQuery
    {
        IApiRequestQuery WithParameters(params object[] parameters);
        IApiRequestQuery WithParameter(object parameter);
    }
}
