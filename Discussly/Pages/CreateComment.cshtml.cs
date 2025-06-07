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

namespace Discussly.Pages
{
    public class CreateCommentModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly UserManager<DiscusslyUser> _userManager;

        public CreateCommentModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            UserManager<DiscusslyUser> userManager)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public CommentInputViewModel Input { get; set; } = new();

        public void OnGet(string parentType, int parentId)
        {
            if(parentType == "Post")
            {
            Input.ParentType = CommentType.Post;
            }
            if(parentType == "Comment")
            {
            Input.ParentType = CommentType.Comment;
            }

            Input.ParentId = parentId;
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
            if(comment.ParentType == CommentType.Post)
            {
            return RedirectToPage("/Post", new { id = comment.ParentId });
            }
            return RedirectToPage("/Index");
        }
    }
}
