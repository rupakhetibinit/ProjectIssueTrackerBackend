using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Repositories.Contracts;

namespace ProjectIssueTracker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApiDBContext _dbContext;
        private readonly IMapper _mapper;

        public ProjectService(ApiDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Project> CreateProject(ProjectCreateDto project)
        {
            var newProject = _mapper.Map<Project>(project);

            await _dbContext.Projects.AddAsync(newProject);

            await _dbContext.SaveChangesAsync();

            return newProject;
        }


        public async Task<IEnumerable<Project>> GetOwnedProjectsAsync(int userId, int pageNumber = 1, int pageSize = 9)
        {

            var result = await _dbContext.Projects
                .Include(p => p.Issues)
                .AsSplitQuery()
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId)
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task DeleteProject(Project project)
        {
            var pro = await _dbContext.Projects
                .Include(p => p.Issues)
                .Include(p => p.Collaborators)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            if (pro == null)
            {
                return;
            }

            foreach (var issue in project.Issues)
            {
                _dbContext.Issues.Remove(issue);
            }

            foreach (var collaborators in project.Collaborators)
            {
                _dbContext.ProjectCollaborators.Remove(collaborators);
            }

            _dbContext.Projects.Remove(pro);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int projectId, bool includeCollaborators, bool includeIssues)
        {
            var projects = _dbContext.Projects.AsQueryable<Project>();

            if (includeCollaborators)
            {
                projects = projects.Include(p => p.Collaborators).ThenInclude(p => p.User);
            }

            if (includeIssues)
            {
                projects = projects.Include(p => p.Issues);
            }

            return await projects.Include(p => p.Owner)
                .FirstOrDefaultAsync(project => project.Id == projectId);
        }

        public async Task<Project> UpdateProject(ProjectUpdateDto updateProject, Project project)
        {
            var result = await _dbContext.Projects.FindAsync(project.Id);

            _mapper.Map(updateProject, project);
            await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<ProjectCollaborator> GetCollaborator(int userId, int projectId)
        {
            var collaborator = await _dbContext.ProjectCollaborators
                .FirstOrDefaultAsync(project => project.ProjectId == projectId && project.UserId == userId);

            return collaborator;
        }
    }
}
