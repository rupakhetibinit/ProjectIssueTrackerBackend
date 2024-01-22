namespace ProjectIssueTracker.Dtos.ResponseDtos
{
    public class CollaboratorDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int ProjectId { get; set; }
    }
}
