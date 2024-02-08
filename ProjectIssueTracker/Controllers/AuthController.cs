using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Features.Authentication.Login;
using ProjectIssueTracker.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NuGet.Protocol.Plugins;
using ProjectIssueTracker.Features.Authentication.Register;
using ISender = MediatR.ISender;

namespace ProjectIssueTracker.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApiDBContext _context;

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        private readonly string _imageDirectory;
        private readonly ISender _sender;

        public AuthController(ApiDBContext context, IConfiguration config, IMapper mapper, IWebHostEnvironment env,ISender sender)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _env = env;
            _imageDirectory = env.WebRootPath + @"\Images";
            _sender = sender;
        }
        [HttpPost("login")]
        public async  Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var result = await _sender.Send(new LoginCommand() { Email = userLoginDto.Email, Password=userLoginDto.Password});
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto user)
        {
            var registerCommand = await _sender.Send(new RegisterCommand(){Email = user.Email,Name = user.Name,Password = user.Password});
            if (registerCommand.IsFailure)
            {
                return BadRequest(registerCommand.Error);
            }

            return Ok(registerCommand.Value);
        }

        [HttpPost("image-upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file,[FromForm]string email)
        {
            
            var filePath = $"{_imageDirectory}//{file.FileName}";

            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { filePath });
        }
        private string GenerateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Secret").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expiretime = DateTime.Now.AddDays(7);
            var token = new JwtSecurityToken(claims: claims, expires: expiretime, signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            CookieOptions cookieOptions = new()
            {
                Expires = expiretime,
                Secure = true
            };

            Response.Cookies.Append("jwt-token", jwt, cookieOptions);

            return jwt;
        }

    }
}
