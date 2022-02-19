using System;

// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Http.Content
{
    public static class ContentTypeExtensions
    {
        public static string ToTypeString(this ContentType mediaType)
        {
            return mediaType switch
            {
                ContentType.Json => "application/json",
                ContentType.TextXml => "text/xml",
                ContentType.FormUrlEncoded => "application/x-www-form-urlencoded",
                _ => throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null)
            };
        }
    }
}