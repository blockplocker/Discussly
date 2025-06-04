using Discussly.Areas.Identity.Data;
using Discussly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Discussly.Pages.Admin.PostAdmin
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly UserManager<DiscusslyUser> _userManager;

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        [BindProperty]
        public Post Post { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/posts/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var post = await response.Content.ReadFromJsonAsync<Post>();
            if (post == null)
                return NotFound();

            Post = post;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/posts/{Post.Id}", Post);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update post via API.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
