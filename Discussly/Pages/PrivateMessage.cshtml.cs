using System.Security.Claims;
using Discussly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Discussly.Data;
using Discussly.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Discussly.Pages
{
    public class PrivateMessageModel : PageModel
    {
        private readonly DiscusslyContext _context;
        private readonly UserManager<DiscusslyUser> _userManager;

        public PrivateMessageModel(DiscusslyContext context, UserManager<DiscusslyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string? CurrentUserId { get; set; }
        public string? OtherUserId { get; set; }
        public List<PrivateMessage> Messages { get; set; } = new();
        public DiscusslyUser? OtherUser { get; set; }

        [BindProperty]
        public string NewMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            OtherUserId = userId;

            if (CurrentUserId == null || string.IsNullOrEmpty(OtherUserId))
                return NotFound();

            Messages = await _context.PrivateMessages
                .Where(m =>
                    (m.SenderId == CurrentUserId && m.ReceiverId == OtherUserId) ||
                    (m.SenderId == OtherUserId && m.ReceiverId == CurrentUserId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            OtherUser = await _userManager.FindByIdAsync(OtherUserId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId)
        {
            CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            OtherUserId = userId;

            if (CurrentUserId == null || string.IsNullOrEmpty(OtherUserId) || string.IsNullOrWhiteSpace(NewMessage))
                return RedirectToPage(new { userId = OtherUserId });

            var message = new PrivateMessage
            {
                Content = NewMessage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SenderId = CurrentUserId,
                ReceiverId = OtherUserId,
                IsRead = false
            };

            _context.PrivateMessages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { userId = OtherUserId });
        }
    }
}
