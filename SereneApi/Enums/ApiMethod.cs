namespace DeltaWare.SereneApi.Enums
{
    /// <summary>
    /// The API Method in which the RESTful request will be made.
    /// </summary>
    public enum ApiMethod
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
