using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public interface IUserService
    {
        Task<User?> GetUserById(int id);
    }
}
