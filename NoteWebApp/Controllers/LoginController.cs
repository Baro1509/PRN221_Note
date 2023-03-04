using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoteWebApp.Request;
using Repository;
using Repository.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteWebApp.Controllers {
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _config;
        
        public LoginController(UserRepository userRepository, IConfiguration config) {
            _userRepository = userRepository;
            _config = config;
        }

        [HttpPost]
        public IActionResult Login([FromBody]LoginRequest loginRequest) {
            if (loginRequest == null || loginRequest.Email.IsNullOrEmpty() || loginRequest.Password.IsNullOrEmpty()) {
                return BadRequest();
            }
            var user = _userRepository.GetAll().Where(p => p.Email == loginRequest.Email && p.Password == loginRequest.Password).FirstOrDefault();
            if (user == null) {
                return Unauthorized();
            }
            var tokenString = GenerateJSONWebToken(user);
            return Ok(new {token =  tokenString});
        }

        private string GenerateJSONWebToken(User user) {
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
    }
}
