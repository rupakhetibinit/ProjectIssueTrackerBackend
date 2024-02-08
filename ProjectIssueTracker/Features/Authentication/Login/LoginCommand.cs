using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Services;
using ProjectIssueTracker.Shared;

namespace ProjectIssueTracker.Features.Authentication.Login
{
    public class LoginCommand : IRequest<Result<LoginCommandResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal class LoginCommandResponse
    {
        public UserDto User { get; set; }
        public string token { get; set; }
    }

    internal sealed class LoginCommandHandler(ApiDBContext dBContext, IMapper mapper, IJwtProvider jwt)
        : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
    {
        public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var foundUser = await dBContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            
            if (foundUser is null)
            {
                return Result.Failure<LoginCommandResponse>(new Error("User.CredentialsInvalid", "Invalid credentials"));
            }
            var passwordMatch = BCrypt.Net.BCrypt.Verify(request.Password, foundUser.Password);
            
            if (!passwordMatch)
            {
                return Result.Failure<LoginCommandResponse>(new Error("User.CredentialsInvalid", "Password does not match"));
            }

            var token = jwt.Generate(foundUser);
            
            return Result.Success(new LoginCommandResponse { User = mapper.Map<UserDto>(foundUser), token = token });
        }

    }
}
