using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository;
namespace NoteWebApp.Controllers

{
    [Route("api/notes")]
    [ApiController]
    public class NoteController : Controller
    {
        private readonly NoteRepository _noteRepository;
        private readonly CardRepository _cardRepository;
        private readonly IMapper _mapper;

        public NoteController(NoteRepository noteRepository, IMapper mapper)
        {
            _noteRepository = noteRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetNoteMenu")]
        [ActionName("GetNoteMenu")]
        public IActionResult Get()
        {
            var user = HttpContext.User;
            if (user == null)
            {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId"))
            {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var noteMenu = _noteRepository.GetAll().Where(o => o.UserId == userid).Select(p => _mapper.Map<NoteMenuResponse>(p)).ToList();
            return Ok(noteMenu);
        }
        [HttpGet]
        [ActionName("GetOneNote")]
        [Route("/api/notes/{noteId:guid}")]
        public IActionResult GetTask(Guid noteId)
        {
            var user = HttpContext.User;
            if (user == null)
            {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId"))
            {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var note = _noteRepository.GetAll().Where(p => p.UserId == userid && p.Id == noteId).Include(p => p.Cards).Select(p => _mapper.Map<NoteCardResponse>(p)).FirstOrDefault();
            return Ok(new { note = note });
        }
        [HttpPost]
        public IActionResult Create([FromBody] NoteRequest note)
        {
            var user = HttpContext.User;
            if (user == null)
            {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId"))
            {
                return Unauthorized();
            }

            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            if (userid != note.UserId)
            {
                return Unauthorized(new
                {
                    message = "You are now allowed to create note for this user"
                });
            }

            var noteInDB = _noteRepository.GetAll().Where(p => p.UserId == userid && p.Id == note.Id).FirstOrDefault();
            if (noteInDB != null)
            {

            }
            note.CreatedAt = DateTime.Now;
            note.UpdatedAt = DateTime.Now;
            _noteRepository.Create(_mapper.Map<Repository.Models.Note>(note));
            return Ok(new
            {
                message = "Note created successfully"
            });
        }
        [HttpPut]
        public IActionResult Update([FromBody] NoteRequest note)
        {
            var user = HttpContext.User;
            if (user == null)
            {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId"))
            {
                return Unauthorized();
            }

            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            if (userid != note.UserId)
            {
                return Unauthorized(new
                {
                    message = "You are now allowed to update note for this user"
                });
            }

            var noteInDB = _noteRepository.GetAll().Where(p => p.UserId == userid && p.Id == note.Id).FirstOrDefault();
            if (noteInDB == null)
            {
                return NotFound(new
                {
                    message = "The note you are trying to update is not available"
                });
            }

            noteInDB.Title = note.Title;
            noteInDB.UpdatedAt = DateTime.Now;

            _noteRepository.Update(noteInDB);
            return Ok(new
            {
                message = "Note updated successfully"
            });
        }
        [HttpDelete]
        public IActionResult Delete([FromQuery] Guid id)
        {
            var user = HttpContext.User;
            if (user == null)
            {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId"))
            {
                return Unauthorized();
            }

            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var noteInDB = _noteRepository.GetAll().Where(p => p.UserId == userid && p.Id == id).FirstOrDefault();
            if (noteInDB == null)
            {
                return NotFound(new
                {
                    message = "The note you are trying to delete is not available"
                });
            }
            if (userid != noteInDB.UserId)
            {
                return Unauthorized(new
                {
                    message = "You are not allowed to delete note for this user"
                });
            }
            _noteRepository.Delete(noteInDB);
            return Ok(new
            {
                message = "Note deleted successfully"
            });
        }


    }
}
