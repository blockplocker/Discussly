using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Discussly.Data;
using Discussly.Models;

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


        public IList<Comment> Comment { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Comment = await _httpClient.GetFromJsonAsync<List<Comment>>($"{_apiBaseUrl}/api/comments") ?? new List<Comment>();
        }
    }
}
