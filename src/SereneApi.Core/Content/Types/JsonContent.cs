﻿using SereneApi.Core.Media;
using System;
using System.Net.Http;
using System.Text;

namespace SereneApi.Core.Content.Types
{
    public class JsonContent : IRequestContent
    {
        public string Content { get; }

        /// <summary>
        /// The <see cref="Encoding"/> of the in body content.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// The <see cref="MediaType"/> of the in body content.
        /// </summary>
        public MediaType MediaType { get; }

        public JsonContent(string content)
        {
            Content = content;
            Encoding = Encoding.UTF8;
            MediaType = MediaType.Json;
        }

        public JsonContent(string content, Encoding encoding, MediaType mediaType)
        {
            Content = content;
            Encoding = encoding;
            MediaType = mediaType;
        }

        public override bool Equals(object obj)
        {
            if (obj is not JsonContent content)
            {
                return false;
            }

            return Equals(Encoding, content.Encoding) && MediaType.Equals(content.MediaType) && Content == content.Content;
        }

        public object GetContent()
        {
            return new StringContent(Content, Encoding, MediaType.GetTypeString());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Encoding, MediaType, Content);
        }
    }
}