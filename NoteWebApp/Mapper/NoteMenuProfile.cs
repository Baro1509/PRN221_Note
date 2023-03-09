using AutoMapper;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository.Models;

namespace NoteWebApp.Mapper
{
    public class NoteMenuProfile : Profile
    {

        public NoteMenuProfile()
        {
            CreateMap<Note, NoteMenuResponse>();
            CreateMap<Note, NoteCardResponse>();
            CreateMap<Card, CardResponse>();
            CreateMap<NoteRequest, Note>()
             .ForMember(dest => dest.Cards, opt => opt.Ignore());
            CreateMap<CardRequest, Card>();
        }

    }
}
