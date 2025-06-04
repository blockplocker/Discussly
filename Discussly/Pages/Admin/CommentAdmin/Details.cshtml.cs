using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Discussly.Models;

namespace Discussly.Pages.Admin.CommentAdmin
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DetailsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        public Comment? Comment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/comments/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            Comment = await response.Content.ReadFromJsonAsync<Comment>();
            if (Comment == null)
                return NotFound();

            return Page();
        }
    }
}
