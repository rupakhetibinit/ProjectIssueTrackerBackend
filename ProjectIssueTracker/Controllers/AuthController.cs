using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Dtos.RequestDtos;
using ProjectIssueTracker.Dtos.ResponseDtos;
using ProjectIssueTracker.Models;
using ProjectIssueTracker.Repositories.Contracts;
using ProjectIssueTracker.Repositories.Repos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public AuthController(ApiDBContext context, IConfiguration config, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _env = env;
            _imageDirectory = env.WebRootPath + @"\Images";
        }
        [HttpPost("login")]
        public IActionResult Login(UserLoginDto userLoginDto)
        {

            var foundUser = _context.Users.FirstOrDefault(u => u.Email == userLoginDto.Email);
            if (foundUser == null)
            {
                return NotFound("Credentials Incorrect");
            }

            var passwordMatch = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, foundUser.Password);

            if (!passwordMatch)
            {
                return NotFound("Credentials Incorrect");
            }

            var token = GenerateToken(foundUser);

            return Ok(new { user = _mapper.Map<UserDto>(foundUser), token });
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegistrationDto user)
        {
            var foundUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (foundUser != null)
            {
                return BadRequest("User already has an account with the same email");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var CreatedUser = new User
            {
                Email = user.Email,
                Password = passwordHash,
                Name = user.Name
            };
            _context.Users.Add(CreatedUser);
            _context.SaveChanges();
            var token = GenerateToken(CreatedUser);

            return Ok(new { user = _mapper.Map<UserDto>(CreatedUser), token });
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
