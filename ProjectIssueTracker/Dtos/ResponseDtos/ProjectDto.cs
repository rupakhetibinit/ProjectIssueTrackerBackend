namespace ProjectIssueTracker.Dtos.ResponseDtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerName { get; set; }
        public int OwnerId { get; set; }
        public List<CollaboratorDto> Collaborators { get; set; }
        public List<IssueDto> Issues { get; set; }
        public Dictionary<string, int> IssueMetrics { get; set; }
    }
    public class IssueMetric
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}
