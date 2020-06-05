// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    /// <summary>
    /// The API Method in which the RESTful request will be made.
    /// </summary>
    public enum Method
    {
        /// <summary>
        /// CREATE
        /// </summary>
        Post,
        /// <summary>
        /// READ
        /// </summary>
        Get,
        /// <summary>
        /// UPDATE / REPLACE
        /// </summary>
        Put,
        /// <summary>
        /// PARTIAL UPDATE / MODIFY
        /// </summary>
        Patch,
        /// <summary>
        /// DELETE
        /// </summary>
        Delete
    }
}
