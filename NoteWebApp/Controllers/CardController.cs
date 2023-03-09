using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository;
using Repository.Models;

namespace NoteWebApp.Controllers
{
    [Route("api/cards")]
    [ApiController]
    public class CardController : Controller
    {
        private readonly CardRepository _cardRepository;
        private readonly NoteRepository _noteRepository;
        private readonly IMapper _mapper;

        public CardController(CardRepository cardRepository, IMapper mapper, NoteRepository noteRepository)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _noteRepository = noteRepository;
        }
        [HttpGet]
        [ActionName("GetNoteWithCard")]
        [Route("/api/cards/{noteId:guid}")]
        public IActionResult GetCard([FromQuery] int orderBy, Boolean isAsc, Guid noteId)
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

            var note = _noteRepository
              .GetAll()
              .Where(p => p.UserId == userid && p.Id == noteId && p.IsDelete == false)
              .Include(p => p.Cards.Where(o => o.IsDelete == false).OrderByDescending(o => o.UpdatedAt))
              .Select(p => _mapper.Map<NoteCardResponse>(p)).FirstOrDefault();
            if (orderBy == 0)
            {
                if (isAsc == true)
                {
                    note.Cards = note.Cards.OrderBy(o => o.UpdatedAt).ToList();

                }
                else
                {
                    note.Cards = note.Cards.OrderByDescending(o => o.UpdatedAt).ToList();
                }
            }
            else if (orderBy == 1)
            {
                if (isAsc == true)
                {
                    note.Cards = note.Cards.OrderBy(o => o.Title).ToList();
                }

                else
                {
                    note.Cards = note.Cards.OrderByDescending(o => o.Title).ToList();

                }
            }

            if (note == null)
            {
                return Ok(new
                {
                    message = "Note not found"
                });
            }

            return Ok(new { note = note });
        }
        [HttpPost]
        public IActionResult Create([FromBody] CardRequest cardRequest)
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
            var note = _noteRepository
                .GetAll()
                .Where(p => p.Id == cardRequest.NoteId && p.UserId == userid && p.IsDelete == false)
                .FirstOrDefault();

            if (note == null)
            {
                return NotFound(new
                {
                    message = "Note not found"
                });
            }

            var card = _cardRepository
                .GetAll()
                .Where(p => p.Id == cardRequest.Id && p.NoteId == cardRequest.NoteId && p.IsDelete == false)
                .FirstOrDefault();
            if (card != null)
            {
                return BadRequest(new
                {
                    message = "Card already exist"
                });
            }

            cardRequest.CreatedAt = DateTime.Now;
            cardRequest.UpdatedAt = DateTime.Now;
            cardRequest.IsDelete = false;
            note.UpdatedAt = DateTime.Now;
            _noteRepository.Update(note);
            _cardRepository.Create(_mapper.Map<Card>(cardRequest));
            return Ok(new
            {
                message = "Card created successfully"
            });
        }
        [HttpPut]
        public IActionResult Update([FromBody] CardRequest cardRequest)
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
            var note = _noteRepository
                .GetAll()
                .Where(p => p.Id == cardRequest.NoteId && p.UserId == userid && p.IsDelete == false)
                .FirstOrDefault();

            if (note == null)
            {
                return NotFound(new
                {
                    message = "Note not found"
                });
            }
            var card = _cardRepository
                .GetAll()
                .Where(p => p.Id == cardRequest.Id && p.NoteId == cardRequest.NoteId && p.IsDelete == false)
                .FirstOrDefault();

            if (card == null)
            {
                return NotFound(new
                {
                    message = "The card you are trying to update is not available"
                });
            }

            card.Title = cardRequest.Title;
            card.Color = cardRequest.Color;
            card.Content = cardRequest.Content;
            card.IsDelete = false;
            card.UpdatedAt = DateTime.Now;
            note.UpdatedAt = DateTime.Now;
            _noteRepository.Update(note);
            _cardRepository.Update(card);
            return Ok(new
            {
                message = "Card updated successfully"
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

            var card = _cardRepository.GetAll().Where(p => p.Id == id && p.IsDelete == false).FirstOrDefault();
            if (card == null)
            {
                return NotFound(new
                {
                    message = "Card item not found"
                });
            }

            var note = _noteRepository
                .GetAll()
                .Where(p => p.Id == card.NoteId && p.IsDelete == false)
                .FirstOrDefault();
            if (note == null)
            {
                return NotFound(new
                {
                    message = "Note not found, denying delete"
                });
            }

            if (note.UserId != userid)
            {
                return Unauthorized(new
                {
                    message = "You are not allowed to delete this note item"
                });
            }
            note.UpdatedAt = DateTime.Now;
            _noteRepository.Update(note);
            card.UpdatedAt = DateTime.Now;
            card.IsDelete = true;
            _cardRepository.Update(card);
            return Ok(new
            {
                message = "Card deleted successfully"
            });
        }
    }

}

