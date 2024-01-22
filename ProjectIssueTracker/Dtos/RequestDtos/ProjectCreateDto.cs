using ProjectIssueTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Dtos.RequestDtos
{
    public class ProjectCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public int OwnerId { get; set; }
    }
}
