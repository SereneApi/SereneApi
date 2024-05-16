namespace Testing
{
    [HttpResource("Users")]
    public interface IUsersApi
    {
        [HttpGetRequest]
        void GetUsersAsync();

        [HttpGetRequest("Search")]
        Task<List<User>> SearchUsersAsync([HttpQuery("first")] string firstName, [HttpQuery] string lastName);

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
