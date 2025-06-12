using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Discussly.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Discussly.Areas.Identity.Data;

namespace Discussly.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly UserManager<DiscusslyUser> _userManager;

        public CategoryModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            UserManager<DiscusslyUser> userManager)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
            _userManager = userManager;
        }

        public Category? Category { get; set; }
        public List<Post> Posts { get; set; } = new();

        public Dictionary<string, UserInfo> UserInfos { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get category details
            Category = await _httpClient.GetFromJsonAsync<Category>($"{_apiBaseUrl}/api/categories/{id}");
            if (Category == null)
            {
                return NotFound();
            }

            // Get all posts and filter by category
            var allPosts = await _httpClient.GetFromJsonAsync<List<Post>>($"{_apiBaseUrl}/api/posts") ?? new List<Post>();
            Posts = allPosts.Where(p => p.CategoryId == id).ToList();

            // Get unique user IDs
            var userIds = Posts.Select(p => p.UserId).Distinct();

            // Fetch user info for each userId
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    UserInfos[userId] = new UserInfo { Name = user.Name, ProfilePic = user.ProfilePic };
                }
                else
                {
                    UserInfos[userId] = new UserInfo { Name = "Unknown", ProfilePic = "NoProfilePic.png" };
                }
            }

            return Page();
        }
    }
}
