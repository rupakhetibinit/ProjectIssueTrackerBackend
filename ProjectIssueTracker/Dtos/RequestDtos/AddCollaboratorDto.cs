using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Dtos.RequestDtos
{
    public class AddCollaboratorDto
    {
        [Required]
        public int UserId { get; set; }
    }
}
