using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Extensions;
using ProjectIssueTracker.Mappings;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public class IssueService : IIssueService
    {
        private readonly IMapper _mapper;
        private readonly ApiDBContext _context;
        public IssueService(IMapper mapper, ApiDBContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IssueDto> DeleteIssue(int issueId)
        {
            var issue = await _context.Issues
                .Include(i => i.Creator)
                .FirstOrDefaultAsync(i => i.Id == issueId);

            _context.Issues.Remove(issue);

            await _context.SaveChangesAsync();
            return _mapper.Map<IssueDto>(issue);

        }

        public async Task<int> GetIssueCountForProject(int projectId)
        {
            var count = await _context.Issues
                .Where(issue => issue.ProjectId == projectId)
                .CountAsync();

            return count;
        }

        public Task<List<IssueDto>> GetIssuesForProject(int projectId, int pageNumber, int pageSize)
        {
            var result = _context.Issues
                    .Where(i => i.ProjectId == projectId)
                    .Include(i => i.Creator)
                    .AsQueryable()
                    .OrderBy(i => i.Id)
                    .Paginate(pageNumber, pageSize)
                    .Items
                    .ToList();

            return Task.FromResult(_mapper.Map<List<IssueDto>>(result));
        }

        public Task<PaginatedResult<Issue>> GetIssuesForUser(int userId, int pageNumber, int pageSize)
        {
            var result = _context.Issues
                .Include(i => i.Project)
                .AsSplitQuery()
                .Include(i => i.Creator)
                .Where(i => i.CreatorId == userId)
                .OrderBy(i => i.Id)
                .Paginate(pageNumber, pageSize);

            return Task.FromResult(result);

        }

        public async Task<IssueDto?> UpdateIssue(int issueId, IssueCreateDto issueUpdateDto)
        {
            var issue = await _context.Issues.FirstOrDefaultAsync(i => i.Id == issueId);

            if (issue == null)
            {
                return null;
            }

            issue.Title = issueUpdateDto.Title;
            issue.Status = issueUpdateDto.Status;
            issue.Description = issueUpdateDto.Description;

            await _context.SaveChangesAsync();

            return _mapper.Map<IssueDto>(issue);
        }

        public async Task CreateIssue(int projectId, int userId, IssueCreateDto issueCreate)
        {
            await _context.Issues.AddAsync(new Issue { Title = issueCreate.Title, Description = issueCreate.Description, CreatorId = userId, Status = issueCreate.Status, ProjectId = projectId });
            await _context.SaveChangesAsync();
        }
    }
}
