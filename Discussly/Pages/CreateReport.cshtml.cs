using Discussly.Areas.Identity.Data;
using Discussly.Data;
using Discussly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Discussly.Pages
{
    public class CreateReportModel : PageModel
    {
        private readonly DiscusslyContext _context;
        private readonly UserManager<DiscusslyUser> _userManager;

        public CreateReportModel(DiscusslyContext context, UserManager<DiscusslyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public ReportType ReportedType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReportedId { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500, ErrorMessage = "Reason must be at most 500 characters.")]
        public string Reason { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(ReportedId))
                return BadRequest();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var report = new Report
            {
                Reason = Reason,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                ReportedId = ReportedId,
                ReportedType = ReportedType,
                Status = Status.Pending
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            TempData["ReportSuccess"] = "Report submitted successfully.";
            
            return RedirectToPage("/Index");
        }
    }
}
