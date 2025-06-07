using Microsoft.AspNetCore.Mvc;

namespace Discussly.Models
{
    public class PostInputViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = null;
        public int CategoryId { get; set; } = 0;
    }
}
