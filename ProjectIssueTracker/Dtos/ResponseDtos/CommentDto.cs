using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Dtos.ResponseDtos
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CommenterId { get; set; }
        public int CommenterName { get; set; }
        public int IssueId { get; set; }
    }
}
