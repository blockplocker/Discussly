using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Discussly.Data;
using Discussly.Models;

namespace Discussly.Pages.Admin.PostAdmin
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

        public IList<Post> Post { get; set; } = new List<Post>();

        public async Task OnGetAsync()
        {
            Post = await _httpClient.GetFromJsonAsync<List<Post>>($"{_apiBaseUrl}/api/posts") ?? new List<Post>();
        }
    }
}
