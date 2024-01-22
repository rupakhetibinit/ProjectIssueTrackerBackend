using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIssueTracker.Notifications
{
    public class IssueHubService
    {
        private readonly IHubContext<IssueHub, IIssueHub> _hubContext;
        private Dictionary<string, UserInfo> _connectedUsers = new();

        public IssueHubService(IHubContext<IssueHub, IIssueHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void AddUser(string connectionId, UserInfo userInfo)
        {
            _connectedUsers[connectionId] = userInfo;
        }
        public void RemoveUser(string connectionId)
        {
            _connectedUsers.Remove(connectionId);
        }

        public string GetConnectionIdByUser(UserInfo userInfo)
        {
            return _connectedUsers.FirstOrDefault(pair => pair.Value == userInfo).Key;
        }

        public async Task NotifyUser(int userId, string projectName)
        {
            foreach (var connectedUser in _connectedUsers.Values)
            {
                if (connectedUser.UserId == userId.ToString())
                {
                    var connectionId = GetConnectionIdByUser(connectedUser);
                    if (connectionId != null)
                    {
                        await _hubContext.Clients.Client(connectionId).CollaboratorUpdate($"You were just added as a collaborator to a new project: {projectName}");
                    }

                }
            }
        }

        public async Task UpdateToOwner(int userId, string projectName)
        {
            foreach (var connectedUser in _connectedUsers.Values)
            {
                if (connectedUser.UserId == userId.ToString())
                {
                    var connectionId = GetConnectionIdByUser(connectedUser);
                    if (connectionId != null)
                    {
                        await _hubContext.Clients.Client(connectionId).OwnerNotification($"A new issue was added to your project {projectName}.");
                    }

                }
            }
        }


        public async Task NotifyRemoval(int userId, string projectName)
        {
            foreach (var connectedUser in _connectedUsers.Values)
            {
                if (connectedUser.UserId == userId.ToString())
                {
                    var connectionId = GetConnectionIdByUser(connectedUser);
                    if (connectionId != null)
                    {
                        await _hubContext.Clients.Client(connectionId).CollaboratorRemove($"You were just removed as a collaborator from the project: {projectName}");
                    }

                }
            }
        }


        public async Task NotifyIssueUpdate(int issueId)
        {
            foreach (var connectedUser in _connectedUsers.Values)
            {
                if (connectedUser.SubscribedIssues.Contains(issueId.ToString()))
                {
                    var connectionId = GetConnectionIdByUser(connectedUser);

                    if (connectionId != null)
                    {
                        await _hubContext.Clients.Client(connectionId).IssueUpdate($"Your issue with Id {issueId} was just updated");
                    }
                }
            }
        }

    }
}