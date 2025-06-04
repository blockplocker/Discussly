namespace Discussly.Models
{
    public class Report
    {
        public int Id { get; set; }
        public required string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string UserId { get; set; }
        public required string ReportedId { get; set; }
        public ReportType ReportedType { get; set; } // "Post", "Comment", "User", or "PrivateMessage"
        public Status Status { get; set; } // "Pending", "UnderReview", or "Resolved"
    }
}
