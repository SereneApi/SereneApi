using SereneApi.Resource;
using SereneApi.Resource.Source;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var registry = new ApiResourceProvider(new AssemblyApiResourceCollection());

            IUsersApi userApi = registry.CreateResourceHandler<IUsersApi>();

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            userApi.GetUserStreamAsync(tokenSource.Token);
            //userApi.GetUsersAsync("john", "smith");
        }
    }
}