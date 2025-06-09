using Microsoft.AspNetCore.Mvc.RazorPages;
using Discussly.Models;

namespace Discussly.Pages.Admin.PostAdmin
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        public IList<Post> Posts { get; set; } = new List<Post>();
        public string? ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Posts");
                if (response.IsSuccessStatusCode)
                {
                    var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
                    if (posts != null)
                    {
                        Posts = posts;
                    }
                }
                else
                {
                    ErrorMessage = "Failed to load Posts from the API.";
                }
            }
            catch
            {
                ErrorMessage = "An error occurred while loading Posts.";
            }
        }
    }
}
