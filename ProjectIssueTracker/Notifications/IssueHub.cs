using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ProjectIssueTracker.Notifications;

public class UserInfo
{
    public string UserId { get; set; }
    public List<string> SubscribedIssues { get; set; } = new List<string>();
}

[Authorize]
public class IssueHub : Hub<IIssueHub>
{
    private readonly IssueHubService _issueHubService;
    private readonly ApiDBContext _dbContext;

    public IssueHub(IssueHubService issueHubService, ApiDBContext dbContext)
    {
        _issueHubService = issueHubService;
        _dbContext = dbContext;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

        var issues = _dbContext.Issues
            .Where(i => i.CreatorId == int.Parse(userId))
            .Select(i => i.Id.ToString())
            .ToList();

        var connectionId = Context.ConnectionId;
        if (connectionId != null && !string.IsNullOrEmpty(connectionId))
        {
            _issueHubService.AddUser(connectionId, new UserInfo { UserId = userId, SubscribedIssues = issues });
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _issueHubService.RemoveUser(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task IssueUpdate(int issueId)
    {
        await Task.FromResult(_issueHubService.NotifyIssueUpdate(issueId));
    }

}

public interface IIssueHub
{
    Task IssueUpdate(string issueId);
    Task CollaboratorUpdate(string message);
    Task CollaboratorRemove(string message);
    Task OwnerNotification(string message);
}
