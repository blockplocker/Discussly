namespace Discussly.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string SenderId { get; set; } 
        public required string ReceiverId { get; set; } 
        public bool IsRead { get; set; } 
    }
}
