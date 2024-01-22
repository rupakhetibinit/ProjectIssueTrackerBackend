using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Extensions;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Services
{
    public interface IIssueService
    {
        Task<IssueDto> DeleteIssue(int issueId);
        Task<List<IssueDto>> GetIssuesForProject(int projectId,int pageNumber,int pageSize);
        Task<PaginatedResult<Issue>> GetIssuesForUser(int userId,int pageNumber,int pageSize);
        Task<IssueDto?> UpdateIssue(int issueId,IssueCreateDto issueUpdateDto);
        Task<int> GetIssueCountForProject(int projectId);
        Task CreateIssue(int projectId, int userId, IssueCreateDto createIssue);
    }
}
