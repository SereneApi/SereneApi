#if !RELEASE
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SereneApi.Tests")]
#endif
namespace SereneApi.Helpers
{
    /// <summary>
    /// Contains all messages used by SereneApi.
    /// </summary>
    internal class MessageHelper
    {
        public const string RequestTimedOutRetryLimit = "The Request Timed Out; Retry limit reached";
    }
}
