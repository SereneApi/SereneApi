// ReSharper disable once CheckNamespace
namespace SereneApi.Core.Http.Responses
{
    public static class StatusExtensions
    {
        public static bool IsSuccessCode(this Status status)
        {
            return status is Status.Ok or Status.Created or Status.Accepted or Status.NoContent;
        }
    }
}