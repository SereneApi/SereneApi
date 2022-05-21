using SereneApi.Core.Http.Responses;

namespace SereneApi.Extensions.Mocking.Rest.Handler
{
    public abstract class MockRestApiHandlerBase
    {
        /// <summary>
        /// Returns a response with a <see cref="Status"/> of Ok.
        /// </summary>
        protected IMockResult Ok()
        {
            return StatusCode(Status.Ok);
        }

        /// <summary>
        /// Returns a response with a <see cref="Status"/> of Ok.
        /// </summary>
        /// <param name="result">The result to be appended to the body of a response.</param>
        protected IMockResult Ok(object result)
        {
            return StatusCode(Status.Ok, result);
        }

        /// <summary>
        /// Returns a response with a <see cref="Status"/> of Not Found.
        /// </summary>
        protected IMockResult NotFound()
        {
            return StatusCode(Status.NotFound);
        }

        /// <summary>
        /// Returns a response with a <see cref="Status"/> of Not Found.
        /// </summary>
        /// <param name="result">The result to be appended to the body of a response.</param>
        protected IMockResult NotFound(object result)
        {
            return StatusCode(Status.NotFound, result);
        }

        /// <summary>
        /// Returns a response with a <see cref="Status"/> of Bad Request.
        /// </summary>
        protected IMockResult BadRequest()
        {
            return StatusCode(Status.BadRequest);
        }

        /// <summary>
        /// Returns a response with a <see cref="Status"/> of Bad Request.
        /// </summary>
        /// <param name="result">The result to be appended to the body of a response.</param>
        protected IMockResult BadRequest(object result)
        {
            return StatusCode(Status.BadRequest, result);
        }

        /// <summary>
        /// Returns a response with a <see cref="Status"/> of No Content.
        /// </summary>
        protected IMockResult NoContent()
        {
            return StatusCode(Status.NoContent);
        }

        /// <summary>
        /// Returns a response with a <see cref="Status"/> of No Content.
        /// </summary>
        /// <param name="result">The result to be appended to the body of a response.</param>
        protected IMockResult NoContent(object result)
        {
            return StatusCode(Status.NoContent, result);
        }

        /// <summary>
        /// Returns a response with the specified <see cref="Status"/>
        /// </summary>
        /// <param name="status">The status to be returned by the response.</param>
        protected IMockResult StatusCode(Status status)
        {
            return new MockStatusResult(status);
        }

        /// <summary>
        /// Returns a response with specified <see cref="Status"/>.
        /// </summary>
        /// <param name="status">The status to be returned by the response.</param>
        /// <param name="result">The result to be appended to the body of a response.</param>
        protected IMockResult StatusCode(Status status, object result)
        {
            return new MockObjectResult(status, result);
        }
    }
}
