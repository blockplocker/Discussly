namespace Discussly.Models
{
    public class CommentInputViewModel
    {
        public string Content { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public CommentType ParentType { get; set; }
    }
}
