
namespace SereneApi.Abstractions.Queries
{
    /// <summary>
    /// Formats objects to strings.
    /// </summary>
    /// <param name="queryObject">The object to be formatted to a string.</param>
    internal delegate string ObjectToStringFormatter(object queryObject);
}
