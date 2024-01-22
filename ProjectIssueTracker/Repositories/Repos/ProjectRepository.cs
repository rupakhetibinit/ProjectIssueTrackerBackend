using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Repositories.Contracts;

namespace ProjectIssueTracker.Repositories.Repos
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApiDBContext dbContext) : base(dbContext)
        {
            
        }

    }
}
