using Microsoft.AspNetCore.Mvc;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetOwnedProjectsAsync(int userId, int pageNumber = 1, int pageSize = 9);

        Task<Project> CreateProject(ProjectCreateDto project);

        Task DeleteProject(Project project);

        Task<Project?> GetProjectByIdAsync(int projectId, bool includeCollaborators, bool includeIssues);

        Task<Project> UpdateProject(ProjectUpdateDto updateProject, Project project);

        Task<ProjectCollaborator> GetCollaborator(int userId, int projectId);
    }
}
