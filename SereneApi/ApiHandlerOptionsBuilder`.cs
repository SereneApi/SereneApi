namespace DeltaWare.SereneApi
{
    /// <summary>
    /// The <see cref="ApiHandlerOptionsBuilder{TApiHandler}"/> is used to build new instances of the <see cref="ApiHandlerOptions{TApiHandler}"/> class
    /// </summary>
    public class ApiHandlerOptionsBuilder<TApiHandler> : ApiHandlerOptionsBuilder where TApiHandler : ApiHandler
    {
        /// <summary>
        /// The <see cref="ApiHandlerOptions{TApiHandler}"/> that will be used by the <see cref="ApiHandler"/>
        /// </summary>
        public new ApiHandlerOptions<TApiHandler> Options => (ApiHandlerOptions<TApiHandler>)base.Options;

        /// <summary>
        /// Instantiates a new instance of the <see cref="ApiHandlerOptionsBuilder{TApiHandler}"/> class
        /// </summary>
        /// <param name="options">The <see cref="ApiHandlerOptions{TApiHandler}"/> to be configured by the <see cref="ApiHandlerOptionsBuilder{TApiHandler}"/></param>
        public ApiHandlerOptionsBuilder(ApiHandlerOptions<TApiHandler> options) : base(options)
        {
        }
    }
}
