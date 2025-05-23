namespace Discussly.Models
{
    public class UserDetails
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public string? ProfilePic { get; set; }
        public string Bio { get; set; }
        public DateTime JoinDate { get; set; }
        public required string Status { get; set; }
        public required string UserId { get; set; }
    }
}
