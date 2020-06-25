using SereneApi.Interfaces;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    public interface IApiHandlerOptions
    {
        /// <summary>
        /// The Dependencies required by the <see cref="ApiHandler"/>.
        /// </summary>
        IDependencyCollection Dependencies { get; }

        IConnectionSettings Connection { get; }
    }
}
