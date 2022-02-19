namespace SereneApi.Core.Http.Content
{
    /// <summary>
    /// The content to be sent in the body of an API request.
    /// </summary>
    public interface IRequestContent
    {
        object GetContent();
    }
}