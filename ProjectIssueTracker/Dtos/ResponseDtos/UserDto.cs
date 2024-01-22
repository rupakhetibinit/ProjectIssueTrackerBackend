namespace ProjectIssueTracker.Dtos.ResponseDtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<ProjectDto> Projects { get; set; }
        public List<ProjectDto> CollaboratedProjects { get; set; }
        public List<CommentDto> Comments { get; set; }
        public List<IssueDto> CreatedIssues { get; set; }
    }
}
