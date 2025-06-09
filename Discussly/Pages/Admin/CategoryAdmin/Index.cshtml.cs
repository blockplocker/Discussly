using Microsoft.AspNetCore.Mvc.RazorPages;
using Discussly.Models;

namespace Discussly.Pages.Admin.CategoryAdmin
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

        public IList<Category> Category { get; set; } = new List<Category>();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/categories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
                    if (categories != null)
                    {
                        Category = categories;
                    }
                }
                else
                {
                    ErrorMessage = "Failed to load categories from the API.";
                }
            }
            catch 
            {
                ErrorMessage = "An error occurred while loading categories.";
            }
        }
    }
}
