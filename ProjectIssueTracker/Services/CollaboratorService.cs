using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly ApiDBContext _dbContext;
        private readonly IMapper _mapper;

        public CollaboratorService(ApiDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddCollaboratorToProjectAsync(Project project, int userId)
        {
            await _dbContext.ProjectCollaborators.AddAsync(new ProjectCollaborator { ProjectId = project.Id, UserId = userId });
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveCollaboratorFromProjectAsync(ProjectCollaborator projectCollaborator)
        {
            _dbContext.Remove(projectCollaborator);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> SearchCollaboratorByNameOrEmail(int projectId, string searchQuery)
        {
            var search = searchQuery.ToLower();

            var collaborators = await _dbContext.ProjectCollaborators.Where(p => p.ProjectId == projectId).Select(pc => pc.UserId).ToListAsync();


            var usersExceptCollaborators = await _dbContext.Users
                .Where(c => c.Name.ToLower().Contains(search) || c.Email.ToLower().Contains(search))
                .Where(c => !collaborators.Contains(c.Id))
                .ToListAsync();

            return usersExceptCollaborators;
        }
    }
}
