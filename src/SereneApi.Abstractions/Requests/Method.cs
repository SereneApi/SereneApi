namespace SereneApi.Abstractions.Requests
{
    /// <summary>
    /// The API Method in which the RESTful request will be made.
    /// </summary>
    public enum Method
    {
        /// <summary>
        /// No <see cref="Method"/> selected.
        /// </summary>
        None,
        /// <summary>
        /// The POST method is used to submit an entity to the specified resource, often causing a change in state or side effects on the server.
        /// </summary>
        Post,
        /// <summary>
        /// The GET method requests a representation of the specified resource. Requests using GET should only retrieve data.
        /// </summary>
        Get,
        /// <summary>
        /// The PUT method replaces all current representations of the target resource with the request payload.
        /// </summary>
        Put,
        /// <summary>
        /// The PATCH method is used to apply partial modifications to a resource.
        /// </summary>
        Patch,
        /// <summary>
        /// The DELETE method deletes the specified resource.
        /// </summary>
        Delete
    }
}
