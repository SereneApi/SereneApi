﻿using SereneApi.Core.Responses;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public interface IApiRequestPerformer<TContent>
    {
        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse<TContent>> ExecuteAsync();
    }
}
