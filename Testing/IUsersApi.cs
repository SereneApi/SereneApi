using SereneApi.Resource.Schema.Attributes;
using SereneApi.Resource.Schema.Attributes.Parameter;
using SereneApi.Resource.Schema.Attributes.Request;

namespace Testing
{
    [HttpResource("Users")]
    [HttpVersion("v1")]
    [HttpHeader("auth", "bearer")]
    public interface IUsersApi
    {
        [HttpGetRequest]
        [HttpVersion("v2")]
        Task GetUsersAsync([HttpHeaderParameter("Correlation-Id")] string correlationId);

        [HttpGetRequest("Search")]
        Task<List<User>> SearchUsersAsync([HttpQueryParameter("first")] string firstName, [HttpQueryParameter] string lastName);

        [HttpGetRequest("{id}")]
        Task<User> GetUserAsync(string id);

        [HttpPostRequest("{userId}/Link/Article/{articleId}")]
        Task LinkUserToArticleAsync(string articleId, string userId);

        [HttpPostRequest]
        Task CreateUserAsync([HttpContentParameter] User user);
    }

    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
