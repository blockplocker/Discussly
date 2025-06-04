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

namespace Discussly.Pages.Admin.CommentAdmin
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

        [BindProperty]
        public CommentInputViewModel Input { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userId = _userManager.GetUserId(User) ?? string.Empty;

            var comment = new Comment
            {
                Content = Input.Content,
                ParentId = Input.ParentId,
                ParentType = Input.ParentType,
                UserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Upvotes = 0,
                Downvotes = 0
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/comments", comment);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create comment via API.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
