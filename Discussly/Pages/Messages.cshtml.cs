using System.Security.Claims;
using Discussly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Discussly.Areas.Identity.Data;
using Discussly.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Discussly.Pages
{
    [Authorize]
    public class MessagesModel : PageModel
    {
        private readonly DiscusslyContext _context;
        private readonly UserManager<DiscusslyUser> _userManager;

        public MessagesModel(DiscusslyContext context, UserManager<DiscusslyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string? CurrentUserId { get; set; }
        public List<string> ConversationUserIds { get; set; } = new();
        public Dictionary<string, UserInfo> UserInfos { get; set; } = new();

        public async Task OnGetAsync()
        {
            CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (CurrentUserId == null)
                return;
            
            var messages = await _context.PrivateMessages
                .Where(m => m.SenderId == CurrentUserId || m.ReceiverId == CurrentUserId)
                .ToListAsync();

            ConversationUserIds = messages
                .Select(m => m.SenderId == CurrentUserId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToList();

            foreach (var userId in ConversationUserIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    UserInfos[userId] = new UserInfo { Name = user.Name, ProfilePic = user.ProfilePic };
                }
                else
                {
                    UserInfos[userId] = new UserInfo { Name = "Unknown", ProfilePic = "NoProfilePic.png" };
                }
            }
        }
    }
}
