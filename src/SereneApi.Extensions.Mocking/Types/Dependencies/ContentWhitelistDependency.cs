using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Interfaces;
using System.Collections.Generic;

namespace SereneApi.Extensions.Mocking.Types.Dependencies
{
    public class ContentWhitelistDependency : IWhitelist
    {
        private readonly List<IApiRequestContent> _whitelistedContent;

        public ContentWhitelistDependency(IApiRequestContent content)
        {
            _whitelistedContent = new List<IApiRequestContent> { content };
        }

        public void ExtendWhitelist(IApiRequestContent content)
        {
            _whitelistedContent.Add(content);
        }

        public bool Validate(object value)
        {
            bool validated = true;

            if (value is IApiRequestContent content)
            {
                validated = _whitelistedContent.Contains(content);
            }

            return validated;
        }
    }
}
