using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Core.Http.Content.Types
{
    public class FormUrlEncodedContent : IRequestContent
    {
        public List<KeyValuePair<string, string>> Content { get; }

        public FormUrlEncodedContent(Dictionary<string, string> content)
        {
            Content = content.ToList();
        }

        public override bool Equals(object obj)
        {
            if (obj is not FormUrlEncodedContent content)
            {
                return false;
            }

            return Content == content.Content;
        }

        public object GetContent()
        {
            return new System.Net.Http.FormUrlEncodedContent(Content);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Content);
        }
    }
}