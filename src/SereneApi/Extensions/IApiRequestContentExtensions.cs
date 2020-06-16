using SereneApi.Interfaces;
using SereneApi.Types.Content;
using System;
using System.Net.Http;

namespace SereneApi.Extensions
{
    public static class IApiRequestContentExtensions
    {
        public static StringContent ToStringContent(this IApiRequestContent content)
        {
            if(content is JsonContent jsonContent)
            {
                return new StringContent(jsonContent.Content, jsonContent.Encoding, jsonContent.MediaType.TypeString);
            }

            throw new ArgumentException();
        }
    }
}
