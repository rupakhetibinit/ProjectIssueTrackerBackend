using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using System.Security.Claims;

namespace ProjectIssueTracker.Authorization
{
    public class ProjectOwnershipRequirement : IAuthorizationRequirement
    {
    }

    public class ProjectOwnershipAuthorization : AuthorizationHandler<ProjectOwnershipRequirement>
    {
        private readonly ApiDBContext _dbContext;

        public ProjectOwnershipAuthorization(ApiDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectOwnershipRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;

            if (httpContext != null)
            {

                var routeData = httpContext.GetRouteData();
                if (routeData != null)
                {
                    var userIdClaim =context.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier);

                    if (userIdClaim == null || userIdClaim?.Value == null)
                    {
                        context.Fail();
                        return;
                    }

                    var userId = userIdClaim.Value;

                    var projectId = routeData.Values["projectId"]?.ToString();

                    if (userId != null && !string.IsNullOrEmpty(projectId))
                    {
                        var isOwner = IsUserOwnerOfProject(int.Parse(userId), int.Parse(projectId));
                        if (isOwner)
                        {
                            var _ = isOwner;
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }

        public bool IsUserOwnerOfProject(int userId, int projectId)
        {
            var project = _dbContext.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project != null && project.OwnerId == userId)
            {
                return true;
            }
            return false;
        }
    }
}
