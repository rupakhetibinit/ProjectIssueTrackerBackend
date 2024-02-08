using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Features.Authentication.Login;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Services;
using ProjectIssueTracker.Shared;

namespace ProjectIssueTracker.Features.Authentication.Register;

public class RegisterCommand: IRequest<Result<LoginCommandResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

internal sealed class RegisterCommandHandler(ApiDBContext dbContext,IJwtProvider jwt,IMapper mapper)
    : IRequestHandler<RegisterCommand, Result<LoginCommandResponse>>
{
    public async Task<Result<LoginCommandResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var foundUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);
        if (foundUser is not null)
        {
            return Result.Failure<LoginCommandResponse>(new Error("Register.UserAlreadyExists", "User already exists"));
        }
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var createdUser = new User
        {
            Email = request.Email,
            Password = passwordHash,
            Name = request.Name
        };
        dbContext.Users.Add(createdUser);
        await dbContext.SaveChangesAsync(cancellationToken);
        var token = jwt.Generate(createdUser);
        return Result.Success(new LoginCommandResponse(){User = mapper.Map<UserDto>(createdUser),token = token});
    }
}