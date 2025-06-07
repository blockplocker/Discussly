using System.Net.Http;
using System.Net.Http.Json;
using Discussly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Discussly.Pages
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

        public IList<Category> Categories { get; set; } = new List<Category>();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            //Categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/api/categories") ?? new List<Category>();
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/categories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
                    if (categories != null)
                    {
                        Categories = categories;
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
