namespace ProjectIssueTracker.Dtos.ResponseDtos
{
    public class IssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; } = Status.NotStarted;
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string CreatorEmail { get; set; }
        public int ProjectId { get; set; }
        public List<CommentDto> Comments { get; set; }
    }

    public enum Status
    {
        NotStarted,
        InProgress,
        Completed
    }
}
