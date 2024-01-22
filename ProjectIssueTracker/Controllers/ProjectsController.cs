using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Extensions;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Services;
using System.Drawing.Printing;
using System.Security.Claims;

namespace ProjectIssueTracker.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ApiDBContext _context;
        private readonly IMapper _mapper;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public ProjectsController(ApiDBContext context, IMapper mapper, IProjectService projectService, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _projectService = projectService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProjectForUserAsync([FromBody] ProjectCreateDto project)
        {
            var user = await _userService.GetUserById(project.OwnerId);

            if (user == null)
            {
                return NotFound("User doesn't exist");
            }

            var newProject = await _projectService.CreateProject(project);

            return Ok(_mapper.Map<ProjectDto>(newProject));
        }

        [HttpGet("{projectId}")]
        [Authorize]
        public async Task<IActionResult> GetProjectAsync([FromRoute] int projectId)
        {
            var project = await _projectService.GetProjectByIdAsync(projectId, true, true);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpPut("{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateDto projectUpdate, [FromRoute] int projectId)
        {
            var oldProject = await _projectService.GetProjectByIdAsync(projectId, false, false);

            if (oldProject == null)
            {
                return NotFound("Project not found");
            }

            var newProject = await _projectService.UpdateProject(projectUpdate, oldProject);

            return Ok(_mapper.Map<ProjectDto>(newProject));

        }

        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllProjectForUserAsync([FromRoute] int id, int pageNumber = 1, int pageSize = 9)
        {

            var projects = await _projectService.GetOwnedProjectsAsync(id, pageNumber, pageSize);

            var projectDto = _mapper.Map<List<ProjectDto>>(projects);
            projectDto.ForEach(project => project.IssueMetrics = project.Issues
                .GroupBy(i => i.Status)
                .ToDictionary(g => g.Key.ToString(), g => g.Count()));
            return Ok(projectDto);
        }

        [HttpGet("user/{id}/count")]
        [Authorize]
        public async Task<IActionResult> GetPageCount([FromRoute] int id)
        {
            var totalCount = await _context.Projects
                .Where(p => p.OwnerId == id)
                .CountAsync();


            return Ok(new { count = totalCount });
        }

        [HttpDelete("{projectId}")]
        [Authorize(Policy = "ProjectOwnerPolicy")]
        public async Task<IActionResult> DeleteProjectForuser([FromRoute] int projectId)
        {

            var project = await _projectService.GetProjectByIdAsync(projectId, false, false);

            if (project == null)
            {
                return NotFound();
            }

            await _projectService.DeleteProject(project);

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpGet("collaborations/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCollaborativeProjects([FromRoute] int userId, int pageNumber = 1, int pageSize = 9)
        {
            //var count = _context.Projects
            //    .Include(p => p.Collaborators)
            //    .ThenInclude(p => p.User)
            //    .Include(p => p.Issues)
            //    .Where(p => p.Collaborators.Any(pc => pc.User.Id == userId)).Count();

            var temp = _context.Projects
                .Include(p => p.Collaborators)
                .ThenInclude(p => p.User)
                .Where(p => p.Collaborators.Any(pc => pc.User.Id == userId));


            var count = temp.Count();

            var result = await temp
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return Ok(new { count, projects = _mapper.Map<List<ProjectDto>>(result) });

        }
    }
}
