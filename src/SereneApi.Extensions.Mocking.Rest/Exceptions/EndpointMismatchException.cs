using System;

namespace SereneApi.Extensions.Mocking.Rest.Exceptions
{
    public class EndpointMismatchException : Exception
    {
        public EndpointMismatchException(string[] endpointSections, string[] templateSections) : base($"The endpoint does not match up to the template. Endpoint:[{string.Join('/', endpointSections)}] Template:[{string.Join('/', templateSections)}]")
        {

        }
    }
}
