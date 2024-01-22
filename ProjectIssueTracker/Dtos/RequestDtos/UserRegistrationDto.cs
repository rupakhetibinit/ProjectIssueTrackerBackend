using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectIssueTracker.Dtos.RequestDtos
{
    public class UserRegistrationDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

    }
}
