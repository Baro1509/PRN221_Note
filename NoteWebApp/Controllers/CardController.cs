using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteWebApp.Request;
using Repository;
using Repository.Models;

namespace NoteWebApp.Controllers
{
    [Route("api/notes/cards")]
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
            var note = _noteRepository.GetAll().Where(p => p.Id == cardRequest.NoteId && p.UserId == userid).FirstOrDefault();

            if (note == null)
            {
                return NotFound(new
                {
                    message = "Note not found"
                });
            }

            var card = _cardRepository.GetAll().Where(p => p.Id == cardRequest.Id && p.NoteId == cardRequest.NoteId).FirstOrDefault();
            if (card != null)
            {
                return BadRequest(new
                {
                    message = "Card already exist"
                });
            }

            card.CreatedAt = DateTime.Now;
            card.UpdatedAt = DateTime.Now;
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
            var note = _noteRepository.GetAll().Where(p => p.Id == cardRequest.NoteId && p.UserId == userid).FirstOrDefault();

            if (note == null)
            {
                return NotFound(new
                {
                    message = "Note not found"
                });
            }
            var card = _cardRepository.GetAll().Where(p => p.Id == cardRequest.Id && p.NoteId == cardRequest.NoteId).FirstOrDefault();

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
            card.UpdatedAt = DateTime.Now;

            _cardRepository.Update(card);
            return Ok(new
            {
                message = "Card updated successfully"
            });
        }

    }
}
