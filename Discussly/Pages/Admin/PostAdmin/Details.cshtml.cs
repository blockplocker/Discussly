using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Discussly.Models;
using Discussly.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Discussly.Pages.Admin.PostAdmin
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DetailsModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        public Post? Post { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/posts/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            Post = await response.Content.ReadFromJsonAsync<Post>();
            if (Post == null)
                return NotFound();

            return Page();
        }
    }
}
