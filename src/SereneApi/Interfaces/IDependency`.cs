namespace SereneApi.Interfaces
{
    public interface IDependency<out TDependency> : IDependency
    {
        /// <summary>
        /// The current instance of the <see cref="TDependency"/>.
        /// </summary>
        new TDependency Instance { get; }
    }
}
