using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Discussly.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Discussly.Pages
{
    public class PostModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public PostModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        public Post? Post { get; set; }
        public List<Comment> Comments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get post details
            Post = await _httpClient.GetFromJsonAsync<Post>($"{_apiBaseUrl}/api/posts/{id}");
            if (Post == null)
            {
                return NotFound();
            }

            // Get all comments and filter by post ID
            var allComments = await _httpClient.GetFromJsonAsync<List<Comment>>($"{_apiBaseUrl}/api/comments") ?? new List<Comment>();
            Comments = allComments
                .Where(c => c.ParentType == CommentType.Post && c.ParentId == id)
                .OrderBy(c => c.CreatedAt)
                .ToList();

            return Page();
        }
    }
}
