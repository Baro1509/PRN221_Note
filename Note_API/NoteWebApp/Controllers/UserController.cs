using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteWebApp.Response;
using Repository;

namespace NoteWebApp.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(UserRepository userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetOneUser")]
        public ResponseEntity<UserResponse> Get() {
            var user = _userRepository.GetAll().FirstOrDefault();
            return new ResponseEntity<UserResponse>(_mapper.Map<UserResponse>(user), 200, "Login successful");
        }
    }
}
