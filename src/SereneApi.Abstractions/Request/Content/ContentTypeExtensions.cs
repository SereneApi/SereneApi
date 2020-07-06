using System;

namespace SereneApi.Abstractions.Request.Content
{
    public static class ContentTypeExtensions
    {
        public static string ToTypeString(this ContentType mediaType)
        {
            return mediaType switch
            {
                ContentType.Json => "application/json",
                ContentType.FormUrlEncoded => "application/x-www-form-urlencoded",
                _ => throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null)
            };
        }
    }
}
