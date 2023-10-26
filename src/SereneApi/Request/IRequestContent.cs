namespace SereneApi.Request
{
    public interface IRequestContent
    {
        object Content { get; }

        string ContentType { get; }
    }
}
