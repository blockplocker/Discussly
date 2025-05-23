namespace Discussly.Models
{
    public class Post
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public int CommentsCount { get; set; }
        public required string UserId { get; set; }
        public required int CategoryId { get; set; }
    }
}
