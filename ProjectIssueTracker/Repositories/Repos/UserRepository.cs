using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Repositories.Contracts;

namespace ProjectIssueTracker.Repositories.Repos
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext)
        {
            
        }
    }
}
