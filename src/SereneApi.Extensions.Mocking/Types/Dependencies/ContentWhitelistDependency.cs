using SereneApi.Abstractions.Requests;
using SereneApi.Extensions.Mocking.Enums;
using SereneApi.Extensions.Mocking.Interfaces;
using System.Collections.Generic;

namespace SereneApi.Extensions.Mocking.Types.Dependencies
{
    public class ContentWhitelistDependency: IWhitelist
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

        public Validity Validate(object value)
        {
            if(!(value is IApiRequestContent content))
            {
                return Validity.NotApplicable;
            }

            if(_whitelistedContent.Contains(content))
            {
                return Validity.Valid;
            }

            return Validity.Invalid;
        }
    }
}
