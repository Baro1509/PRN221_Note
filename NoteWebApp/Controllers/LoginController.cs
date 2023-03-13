using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoteWebApp.Request;
using Repository;
using Repository.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteWebApp.Controllers
{
    [Route("api/login")]
    [EnableCors]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public LoginController(UserRepository userRepository, IConfiguration config, IMapper mapper)
        {
            _userRepository = userRepository;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || loginRequest.Email.IsNullOrEmpty() || loginRequest.Password.IsNullOrEmpty())
            {
                return BadRequest();
            }
            var user = _userRepository.GetAll().Where(p => p.Email == loginRequest.Email).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                return Unauthorized();
            }
            var tokenString = GenerateJSONWebToken(user);
            return Ok(new { token = tokenString, userId = user.Id });
        }

        private string GenerateJSONWebToken(User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("Email", user.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                        _config["Jwt:Issuer"],
                        _config["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        [Route("/api/signup")]
        public IActionResult Create([FromBody] UserRequest request) {
            if (!request.validation()) {
                return BadRequest(new {
                    message = "Not a valid request"
                });
            }

            var user = _userRepository.GetAll().Where(p => p.Email == request.Email).FirstOrDefault();
            if (user != null) {
                return BadRequest(new {
                    message = "This email is already used"
                });
            }
            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            request.CreatedAt = DateTime.Now;
            request.UpdatedAt = DateTime.Now;
            _userRepository.Create(_mapper.Map<User>(request));
            var userCheck = _userRepository.GetAll().Where(p => p.Email == request.Email).FirstOrDefault();
            return Ok(user);
        }
    }
}
