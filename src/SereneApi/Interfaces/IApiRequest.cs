namespace SereneApi.Interfaces
{
    public interface IApiRequest
    {
        string EndPoint { get; }

        Method Method { get; }
    }
}
