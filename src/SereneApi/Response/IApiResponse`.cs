namespace SereneApi.Response
{
    public interface IApiResponse<out TResponse> : IApiResponse
    {
        TResponse Response { get; }
    }
}
