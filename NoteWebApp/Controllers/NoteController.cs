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

        public NoteController(CardRepository cardRepository, IMapper mapper, NoteRepository noteRepository)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _noteRepository = noteRepository;
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
            var noteMenu = _noteRepository.GetAll()
                .Where(o => o.UserId == userid && o.IsDelete == false)
                .Select(p => _mapper.Map<NoteMenuResponse>(p)).ToList();
            return Ok(noteMenu);
        }

        [HttpPost]
        public IActionResult Create([FromBody] NoteRequest noteRequest)
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
            if (userid != noteRequest.UserId)
            {
                return Unauthorized(new
                {
                    message = "You are now allowed to create note for this user"
                });
            }

            var noteInDB = _noteRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == noteRequest.Id && p.IsDelete == false)
                .FirstOrDefault();
            if (noteInDB != null)
            {
                return Unauthorized(new
                {
                    message = "Note already exist"
                });
            }
            noteRequest.CreatedAt = DateTime.Now;
            noteRequest.UpdatedAt = DateTime.Now;
            noteRequest.IsDelete = false;
            _noteRepository.Create(_mapper.Map<Repository.Models.Note>(noteRequest));
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

            var noteInDB = _noteRepository.GetAll().Where(p => p.UserId == userid && p.Id == note.Id && p.IsDelete == false).FirstOrDefault();
            if (noteInDB == null)
            {
                return NotFound(new
                {
                    message = "The note you are trying to update is not available"
                });
            }

            noteInDB.Title = note.Title;
            noteInDB.UpdatedAt = DateTime.Now;
            noteInDB.IsDelete = false;
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
            var noteInDB = _noteRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == id && p.IsDelete == false)
                .Include(p => p.Cards.Where(o => o.IsDelete == false))
                .FirstOrDefault();
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
            if (noteInDB.Cards.Count > 0)
            {
                noteInDB.Cards.ToList().ForEach(p =>
                {
                    p.IsDelete = true;
                    p.UpdatedAt = DateTime.Now;
                    _cardRepository.Update(p);
                });
            }

            noteInDB.UpdatedAt = DateTime.Now;
            noteInDB.IsDelete = true;
            _noteRepository.Update(noteInDB);
            return Ok(new
            {
                message = "Note deleted successfully"
            });
        }


    }
}
