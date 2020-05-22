using System;

namespace DeltaWare.SereneApi.DependencyInjection
{
    /// <summary>
    /// The <see cref="ApiHandlerOptions{TApiHandler}"/> to be used by the <see cref="ApiHandler"/> when making API requests
    /// </summary>
    public class ApiHandlerOptions<TApiHandler> : ApiHandlerOptions where TApiHandler : ApiHandler
    {
        #region Public Properties

        public override Type HandlerType => typeof(TApiHandler);

        #endregion
    }
}
