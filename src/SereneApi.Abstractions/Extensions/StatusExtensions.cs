// Do note change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Abstractions.Enums
{
    public static class StatusExtensions
    {
        public static bool IsSuccessCode(this Status status)
        {
            return status == Status.Ok ||
                   status == Status.Created ||
                   status == Status.Accepted ||
                   status == Status.NoContent;
        }
    }
}
