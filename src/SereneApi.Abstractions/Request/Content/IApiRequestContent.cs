namespace SereneApi.Abstractions.Request.Content
{
    /// <summary>
    /// The content to be sent in the body of an <see cref="IApiRequest"/>.
    /// </summary>
    public interface IApiRequestContent
    {
        object GetContent();
    }
}
