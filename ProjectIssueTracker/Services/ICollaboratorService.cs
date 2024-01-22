using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public interface ICollaboratorService
    {
        Task AddCollaboratorToProjectAsync(Project project, int userId);
        Task RemoveCollaboratorFromProjectAsync(ProjectCollaborator projectCollaborator);
        Task<IEnumerable<User>> SearchCollaboratorByNameOrEmail(int projectId, string searchQuery);
    }
}
