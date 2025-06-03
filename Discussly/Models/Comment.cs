namespace Discussly.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public required string UserId { get; set; }
        public required int ParentId { get; set; }
        public required string ParentType { get; set; } // "Post" or "Comment"
    }
}
