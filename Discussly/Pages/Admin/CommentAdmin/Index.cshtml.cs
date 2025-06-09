using Discussly.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Discussly.Pages.Admin.CommentAdmin
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }


        public IList<Comment> Comments { get;set; } = new List<Comment>();
        public string? ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Comments");
                if (response.IsSuccessStatusCode)
                {
                    var comments = await response.Content.ReadFromJsonAsync<List<Comment>>();
                    if (comments != null)
                    {
                        Comments = comments;
                    }
                }
                else
                {
                    ErrorMessage = "Failed to load Comments from the API.";
                }
            }
            catch
            {
                ErrorMessage = "An error occurred while loading Comments.";
            }
        }
    }
}
