using ProjectIssueTracker.Dtos.ResponseDtos;
using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
        public Status Status { get; set; }
    }
}
