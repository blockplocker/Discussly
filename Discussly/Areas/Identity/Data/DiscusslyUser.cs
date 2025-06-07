using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Discussly.Areas.Identity.Data;

// Add profile data for application users by adding properties to the DiscusslyUser class
public class DiscusslyUser : IdentityUser
{
    public required string Name { get; set; }
    [PersonalData]
    public string? ProfilePic { get; set; }
    [PersonalData]
    public string? Bio { get; set; }
    [PersonalData]
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
}

