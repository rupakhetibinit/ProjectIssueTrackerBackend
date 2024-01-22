using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Repositories.Contracts;

namespace ProjectIssueTracker.Repositories.Repos
{
    public class CollaboratorRepository : BaseRepository<ProjectCollaborator>, ICollaboratorRepository
    {
        public CollaboratorRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
