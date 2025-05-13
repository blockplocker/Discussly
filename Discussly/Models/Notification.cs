namespace Discussly.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public required string UserId { get; set; } // The user who received the notification
        public required string RelatedId { get; set; } // The ID of the related post, comment, or user
        public required string Type { get; set; } // "Comment", "Post" or "PrivateMessage".
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
