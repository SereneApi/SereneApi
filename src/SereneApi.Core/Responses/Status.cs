namespace SereneApi.Core.Responses
{
    // https://restfulapi.net/http-status-codes/
    /// <summary>
    /// Species the status of the response from the API.
    /// </summary>
    public enum Status
    {
        #region Supporting

        Unknown = 0,
        None = 1,
        TimedOut = 2,

        #endregion
        #region 2XX - Success

        /// <summary>
        /// 200 - Indicates that the API successfully carried out the requested action.
        /// </summary>
        Ok = 200,

        /// <summary>
        /// 201 - Indicates that the API successfully created the resource.
        /// </summary>
        Created = 201,

        /// <summary>
        /// 202 - Indicates that the API has accepted the request and may take time to process it.
        /// </summary>
        Accepted = 202,

        /// <summary>
        /// 204 - 
        /// </summary>
        NoContent = 204,

        #endregion
        #region 3XX - Redirection

        /// <summary>
        /// 301 - 
        /// </summary>
        MovedPermanently = 301,

        /// <summary>
        /// 302 - 
        /// </summary>
        Found = 302,

        /// <summary>
        /// 303 - 
        /// </summary>
        SeeOther = 303,

        /// <summary>
        /// 304 - 
        /// </summary>
        NotModified = 304,

        /// <summary>
        /// 307 - 
        /// </summary>
        TemporaryRedirect = 307,

        #endregion
        #region 4XX - Client Error

        /// <summary>
        /// 400 - 
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// 401 - 
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 403
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 404
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 405
        /// </summary>
        MethodNotAllowed = 405,

        /// <summary>
        /// 406
        /// </summary>
        NotAcceptable = 406,

        /// <summary>
        /// 412
        /// </summary>
        PreconditionFailed = 412,

        /// <summary>
        /// 415
        /// </summary>
        UnsupportedMediaType = 415,

        #endregion
        #region 5XX - Server Error

        /// <summary>
        /// 500
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// 501
        /// </summary>
        NotImplemented = 501,

        #endregion
    }
}
