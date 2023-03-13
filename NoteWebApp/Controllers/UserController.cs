using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository;
using Repository.Models;

namespace NoteWebApp.Controllers {
    [Authorize]
    [Route("api/users")]
    [ApiController]
    [EnableCors]
    public class UserController : ControllerBase {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(UserRepository userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //[HttpGet(Name = "GetOneUser")]
        //public ResponseEntity<UserResponse> GetOneUser() {
        //    var user = _userRepository.GetAll().FirstOrDefault();
        //    return new ResponseEntity<UserResponse>(_mapper.Map<UserResponse>(user), 200, "Get user successfully");
        //}
        
        //[HttpGet(Name = "GetAllUsers")]
        //[ActionName("GetAllUsers")]
        //public ResponseEntity<List<UserResponse>> Get() {
        //    var users = _userRepository.GetAll().Select(p => _mapper.Map<UserResponse>(p)).ToList();
        //    return new ResponseEntity<List<UserResponse>>(users, 200, "Get all users successfully");
        //}
        
        [HttpGet]
        //TODO: Hash password
        public IActionResult Get() {
            var users = _userRepository.GetAll().Select(p => _mapper.Map<UserResponse>(p)).ToList();
            return Ok(users);
        }
    }
}
