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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discussly.Pages.Admin.PostAdmin
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

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/api/categories");
            CategoryOptions = categories?
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList() ?? new List<SelectListItem>();

            return Page();
        }

        [BindProperty]
        public PostInputViewModel Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var Post = new Post
            {
                Title = Input.Title,
                Content = Input.Content,
                ImageUrl = Input.ImageUrl,
                CategoryId = Input.CategoryId,
                UserId = _userManager.GetUserId(User) ?? string.Empty,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Upvotes = 0,
                Downvotes = 0,
                CommentsCount = 0,
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/posts", Post);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create post via API.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
