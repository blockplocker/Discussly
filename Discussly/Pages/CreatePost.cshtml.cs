using Discussly.Areas.Identity.Data;
using Discussly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Discussly.Pages
{
    [Authorize]
    public class CreatePostModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly UserManager<DiscusslyUser> _userManager;

        public CreatePostModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            UserManager<DiscusslyUser> userManager)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int categoryId)
        {
            Input.CategoryId = categoryId;

            return Page();
        }

        [BindProperty(SupportsGet = true)]
        public PostInputViewModel Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {

            var post = new Post
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

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/posts", post);


            var createdPost = await response.Content.ReadFromJsonAsync<Post>();
            if (createdPost == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to retrieve created post information.");
                return Page();
            }

            return RedirectToPage("/Post", new { id = createdPost.Id });
        }
    }
}
