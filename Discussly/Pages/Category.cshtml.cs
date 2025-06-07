using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Discussly.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;

namespace Discussly.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public CategoryModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        public Category? Category { get; set; }
        public List<Post> Posts { get; set; } = new();

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

            return Page();
        }
    }
}
