using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Features.Authentication.Login;
using ProjectIssueTracker.Features.Authentication.Register;
using ISender = MediatR.ISender;

namespace ProjectIssueTracker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly string _imageDirectory;
        private readonly ISender _sender;

        public AuthController(IWebHostEnvironment env,
            ISender sender)
        {
            _sender = sender;
            _imageDirectory = env.WebRootPath + @"\Images";
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var result = await _sender.Send(new LoginCommand()
                { Email = userLoginDto.Email, Password = userLoginDto.Password });
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto user)
        {
            var registerCommand = await _sender.Send(new RegisterCommand()
                { Email = user.Email, Name = user.Name, Password = user.Password });
            if (registerCommand.IsFailure)
            {
                return BadRequest(registerCommand.Error);
            }

            return Ok(registerCommand.Value);
        }

        [HttpPost("image-upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string email)
        {
            var filePath = $"{_imageDirectory}//{file.FileName}";

            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }

            await using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { filePath });
        }
    }
}