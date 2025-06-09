using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Discussly.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Discussly.Areas.Identity.Data;

namespace Discussly.Pages
{
    public class PostModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly UserManager<DiscusslyUser> _userManager;

        public PostModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            UserManager<DiscusslyUser> userManager)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
            _userManager = userManager;
        }

        public Post? Post { get; set; }
        public List<Comment> Comments { get; set; } = new();
        public Dictionary<string, (string Name, string? ProfilePic)> UserInfos { get; set; } = new();

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

            // Collect all unique user IDs (post + comments)
            var userIds = new HashSet<string> { Post.UserId };
            foreach (var comment in Comments)
            {
                userIds.Add(comment.UserId);
            }

            // Fetch user info for each userId
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    UserInfos[userId] = (user.Name, user.ProfilePic);
                }
                else
                {
                    UserInfos[userId] = ("Unknown", "NoProfilePic.png");
                }
            }

            return Page();
        }
    }
}
