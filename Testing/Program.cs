using SereneApi.Resource;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var registry = ApiResourceRegistry.Create();

            IUsersApi userApi = registry.CreateResourceHandler<IUsersApi>();

            //userApi.LinkUserToArticleAsync("article:1234", "user:1234");
            userApi.GetUsersAsync("john", "smith");
        }
    }
}