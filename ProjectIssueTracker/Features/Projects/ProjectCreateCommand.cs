using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Shared;

namespace ProjectIssueTracker.Features.Projects;

public sealed record ProjectCreateCommand : IRequest<Result<ProjectDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OwnerId { get; set; }
}

public class ProjectCreateValidator : AbstractValidator<ProjectCreateCommand>
{
    public ProjectCreateValidator()
    {
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Description).NotEmpty();
        RuleFor(p => p.OwnerId).NotEmpty();
    }
}

public sealed class ProjectCreateCommandHandler(ApiDBContext dbContext, IMapper mapper,IValidator<ProjectCreateCommand> validator)
    : IRequestHandler<ProjectCreateCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(ProjectCreateCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.OwnerId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<ProjectDto>(new Error("NotFound", "user doesn't exist"));
        }

        var valid = await validator.ValidateAsync(request, cancellationToken);
        if (!valid.IsValid)
        {
            return Result.Failure<ProjectDto>(new Error("BadRequest",valid.Errors.ToString()!));
        }

        var entity = new Project
        {
            Name = request.Name,
            Description = request.Description,
            OwnerId = request.OwnerId
        };
        var newProject = dbContext.Projects.Add(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProjectDto>(entity);
    }
}