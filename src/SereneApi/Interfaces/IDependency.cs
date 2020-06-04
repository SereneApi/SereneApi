using SereneApi.Enums;

namespace SereneApi.Interfaces
{
    public interface IDependency
    {
        /// <summary>
        /// The <see cref="Binding"/> of the <see cref="IDependency"/>.
        /// </summary>
        Binding Binding { get; }
    }
}
