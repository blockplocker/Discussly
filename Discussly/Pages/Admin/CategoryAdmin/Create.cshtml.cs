using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Discussly.Models;
using Discussly.Areas.Identity.Data;

namespace Discussly.Pages.Admin.CategoryAdmin
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly UserManager<DiscusslyUser> _userManager;

        public CreateModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            UserManager<DiscusslyUser> userManager)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CategoryInputViewModel Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Map ViewModel to Entity
            var category = new Category
            {
                Name = Input.Name,
                Description = Input.Description,
                UserId = _userManager.GetUserId(User) ?? string.Empty,
                CreatedAt = DateTime.Now,
                PostsCount = 0
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/categories", category);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create category via API.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
