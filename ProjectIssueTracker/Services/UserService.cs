using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public class UserService : IUserService
    {
        private readonly ApiDBContext _dbContext;

        public UserService(ApiDBContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task<User?> GetUserById(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }
    }
}
