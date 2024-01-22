using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectIssueTracker.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public virtual User Owner{ get; set; }
        public virtual List<Issue> Issues { get; set;}
        public virtual List<ProjectCollaborator> Collaborators { get; set;}
    }
}
