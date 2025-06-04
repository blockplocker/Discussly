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
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DeleteModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/comments/{id}");
            // Optionally, check response.IsSuccessStatusCode and handle errors

            return RedirectToPage("./Index");
        }
    }
}
