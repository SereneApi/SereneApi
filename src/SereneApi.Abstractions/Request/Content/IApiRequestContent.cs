namespace SereneApi.Abstractions.Request.Content
{
    /// <summary>
    /// The content to be sent in the body of an API request.
    /// </summary>
    public interface IApiRequestContent
    {
        object GetContent();
    }
}
