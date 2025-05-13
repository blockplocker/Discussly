namespace Discussly.Models
{
    public class Report
    {
        public int Id { get; set; }
        public required string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string UserId { get; set; }
        public required string ReportedId { get; set; }
        public required string ReportedType { get; set; } // "Post", "Comment", or "User"
        public required string Status { get; set; } // "Pending", "Reviewed", or "Resolved"
    }
}
