using SereneApi.Request.Attributes;
using SereneApi.Request.Attributes.Parameter;
using SereneApi.Request.Attributes.Request;

namespace Testing
{
    [HttpResource]
    public interface IUsersApi
    {
        [HttpGetRequest]
        Task<List<User>> GetUsersAsync();

        [HttpGetRequest]
        Task<List<User>> GetUsersAsync([HttpQuery("first")] string firstName, [HttpQuery] string lastName);

        [HttpGetRequest("{id}")]
        Task<User> GetUserAsync(string id);

        [HttpPostRequest("{userId}/Link/Article/{articleId}")]
        Task LinkUserToArticleAsync(string articleId, string userId);

        [HttpPostRequest]
        Task CreateUserAsync([HttpContent] User user);
    }

    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
