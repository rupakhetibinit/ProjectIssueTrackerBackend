using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Extensions;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Notifications;
using ProjectIssueTracker.Services;

namespace ProjectIssueTracker.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly ApiDBContext _context;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IIssueService _issueService;
        private readonly IHubContext<IssueHub, IIssueHub> _hubContext;
        private readonly IssueHubService _issueHubService;

        public IssuesController(ApiDBContext context, IProjectService projectService, IUserService userService, IMapper mapper, IIssueService issueService, IHubContext<IssueHub, IIssueHub> hubContext,IssueHubService issueHubService)
        {
            _context = context;
            _projectService = projectService;
            _userService = userService;
            _mapper = mapper;
            _issueService = issueService;
            _hubContext = hubContext;
            _issueHubService = issueHubService;
        }

        [HttpPost("{projectId}/issues")]
        [Authorize]
        public async Task<IActionResult> CreateIssueForProjectAsync([FromBody] IssueCreateDto issue, [FromRoute] int projectId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var project = await _projectService.GetProjectByIdAsync(projectId, includeCollaborators: false, includeIssues: true);

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            if (project == null)
            {
                return NotFound();
            }

            await _issueService.CreateIssue(projectId, userId, issue);

            if(project.OwnerId != userId)
            {
                await _issueHubService.UpdateToOwner(project.OwnerId, project.Name);
            }

            

            var response = _mapper.Map<ProjectDto>(project);

            return Ok(response);

        }

        [HttpGet("{projectId}/issues")]
        [Authorize]
        public async Task<IActionResult> GetIssuesForProjectAsync([FromRoute] int projectId, int pageSize, int pageNumber)
        {
            var project = await _projectService.GetProjectByIdAsync(projectId, includeCollaborators: false, includeIssues: true);

            if (project == null)
            {
                return NotFound("Project doesn't exist");
            }

            var issues = await _issueService.GetIssuesForProject(projectId, pageNumber, pageSize);

            return Ok(issues);
        }

        [HttpGet("{projectId}/issues/count")]
        [Authorize]
        public async Task<IActionResult> GetCountOfIssues([FromRoute] int projectId)
        {
            var count = await _issueService.GetIssueCountForProject(projectId);
            return Ok(new { count });
        }

        [HttpDelete("{projectId}/issues/{issueId}")]
        [Authorize]
        public async Task<IActionResult> DeleteIssueById([FromRoute] int projectId, [FromRoute] int issueId)
        {
            //var userIdClaim = HttpContext.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier);
            //var project = await  _projectService.GetProjectByIdAsync(projectId, true, false);
            //if (project.OwnerId != int.Parse(userIdClaim.Value)&&)
            //{
            //    return Forbid();
            //}

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier);
            if (!CheckIfProjectOrIssueOwner(issueId, int.Parse(userIdClaim.Value)))
            {
                return Forbid();
            }

            var issue = await _issueService.DeleteIssue(issueId);

            return Ok(_mapper.Map<IssueDto>(issue));
        }

        [HttpPut("{projectId}/issues/{issueId}")]
        [Authorize]
        public async Task<IActionResult> UpdateIssueById([FromRoute] int issueId, [FromBody] IssueCreateDto updatedIssue)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier)!;

            if (!CheckIfProjectOrIssueOwner(issueId, int.Parse(userIdClaim.Value)))
            {
                return Forbid();
            }


            var issue = await _issueService.UpdateIssue(issueId, updatedIssue);

            if (issue == null)
            {
                return NotFound();
            }

            //await _hubContext.Clients.All.IssueUpdate(issueId);
            await _issueHubService.NotifyIssueUpdate(issueId);

            return Ok(issue);
        }

        [HttpGet("issues")]
        [Authorize]
        public async Task<IActionResult> GetIssuesForUser(int pageSize, int pageNumber)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault((c) => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var issues = await _issueService.GetIssuesForUser(userId, pageNumber, pageSize);
            var issuesDto = _mapper.Map<List<IssueDto>>(issues.Items.ToList());
            return Ok(new { count = issues.TotalCount, issues = issuesDto });
        }
        bool CheckIfProjectOrIssueOwner(int issueId, int userId)
        {
            var issue = _context.Issues
                .Include(c => c.Project)
                .FirstOrDefault(i => i.Id == issueId);

            if (issue.CreatorId == userId || issue.Project.OwnerId == userId)
            {
                return true;
            }
            return false;

        }

    }
}
