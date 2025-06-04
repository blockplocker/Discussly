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
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        [BindProperty]
        public Comment Comment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/comments/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var comment = await response.Content.ReadFromJsonAsync<Comment>();
            if (comment == null)
                return NotFound();

            Comment = comment;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Optionally update the UpdatedAt timestamp
            Comment.UpdatedAt = DateTime.Now;

            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/comments/{Comment.Id}", Comment);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update comment via API.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
