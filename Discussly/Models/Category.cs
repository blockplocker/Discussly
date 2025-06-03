namespace Discussly.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostsCount { get; set; }
        public required string UserId { get; set; } 
    }
}
