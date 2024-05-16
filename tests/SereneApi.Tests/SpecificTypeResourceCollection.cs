using SereneApi.Resource.Source;

namespace SereneApi.Tests
{
    internal sealed class SpecificTypeResourceCollection<T> : IApiResourceCollection where T : class
    {
        public IEnumerable<Type> GetApiResourceTypes()
        {
            yield return typeof(T);
        }
    }
}
