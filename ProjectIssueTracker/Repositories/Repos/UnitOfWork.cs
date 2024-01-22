using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Repositories.Contracts;

namespace ProjectIssueTracker.Repositories.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiDBContext _dbContext;

        public UnitOfWork(ApiDBContext dbContext)
        {
            _dbContext = dbContext;
            Projects = new ProjectRepository(dbContext);
        }

        public IProjectRepository Projects { get; private set; }
        public IUserRepository UserRepository { get; private set; }
        public IIssueRepository IssueRepository { get; private set; }
        public ICollaboratorRepository collaboratorRepository { get; private set; }
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}

