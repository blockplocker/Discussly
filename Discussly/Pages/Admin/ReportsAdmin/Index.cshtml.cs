using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Discussly.Models;
using Discussly.Areas.Identity.Data;
using Discussly.Data;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Discussly.Pages.Admin.ReportsAdmin
{
    public class ReportViewModel
    {
        public Report Report { get; set; } = default!;
        public string UserName { get; set; } = string.Empty;
        public string? ProfilePic { get; set; }
        public Post? Post { get; set; }
        public Comment? Comment { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly DiscusslyContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public IndexModel(
            DiscusslyContext context,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "ApiSettings:BaseUrl configuration is missing.");
        }

        public List<ReportViewModel> Reports { get; set; } = new();

        public async Task OnGetAsync()
        {
            var reports = await _context.Reports.AsNoTracking().ToListAsync();

            var userIds = reports.Select(r => r.UserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var reportViewModels = new List<ReportViewModel>();

            foreach (var r in reports)
            {
                var vm = new ReportViewModel
                {
                    Report = r,
                    UserName = users.TryGetValue(r.UserId, out var user) ? user.Name : "Unknown",
                    ProfilePic = users.TryGetValue(r.UserId, out var user2) ? user2.ProfilePic : null
                };

                // Fetch the reported post or comment from the API
                if (r.ReportedType == ReportType.Post)
                {
                    if (int.TryParse(r.ReportedId, out var postId))
                    {
                        try
                        {
                            vm.Post = await _httpClient.GetFromJsonAsync<Post>($"{_apiBaseUrl}/api/posts/{postId}");
                        }
                        catch { /* handle/log if needed */ }
                    }
                }
                else if (r.ReportedType == ReportType.Comment)
                {
                    if (int.TryParse(r.ReportedId, out var commentId))
                    {
                        try
                        {
                            vm.Comment = await _httpClient.GetFromJsonAsync<Comment>($"{_apiBaseUrl}/api/comments/{commentId}");
                        }
                        catch { /* handle/log if needed */ }
                    }
                }

                reportViewModels.Add(vm);
            }

            Reports = reportViewModels;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int reportId, string reportedId, ReportType reportedType)
        {
            HttpResponseMessage? deleteResponse = null;

            if (reportedType == ReportType.Post)
            {
                deleteResponse = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/posts/{reportedId}");
            }
            else if (reportedType == ReportType.Comment)
            {
                deleteResponse = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/comments/{reportedId}");
            }

            // If the delete succeeded, remove the report from the database
            if (deleteResponse != null && deleteResponse.IsSuccessStatusCode)
            {
                var report = await _context.Reports.FindAsync(reportId);
                if (report != null)
                {
                    //report.Status = Status.Resolved;
                    _context.Reports.Remove(report);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResolveAsync(int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report != null)
            {
                //report.Status = Status.Resolved;
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int reportId, string? reportedId, ReportType reportedType)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report != null)
            {
                report.Status = Status.UnderReview;
                await _context.SaveChangesAsync();

                // Redirect to the correct edit page
                if (reportedType == ReportType.Post && int.TryParse(reportedId, out var postId))
                {
                    return RedirectToPage("/Admin/PostAdmin/Edit", new { id = postId });
                }
                else if (reportedType == ReportType.Comment && int.TryParse(reportedId, out var commentId))
                {
                    return RedirectToPage("/Admin/CommentAdmin/Edit", new { id = commentId });
                }
            }
            // fallback
            return RedirectToPage();
        }
    }
}
