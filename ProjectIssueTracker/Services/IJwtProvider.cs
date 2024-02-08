using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services;

public interface IJwtProvider
{
    public string Generate(User user);
}