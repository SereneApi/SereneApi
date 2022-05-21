using System;

namespace SereneApi.Extensions.Mocking.Rest.Exceptions
{
    public class TemplateParameterNotFoundException : Exception
    {
        public TemplateParameterNotFoundException(string templateParameterName) : base($"The specified template parameter \"{templateParameterName}\" could not be found")
        {
        }
    }
}
