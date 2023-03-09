using AutoMapper;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository.Models;

namespace NoteWebApp.Mapper {
    public class UserProfile : Profile {
        public UserProfile() {
            CreateMap<User, UserResponse>();
            CreateMap<UserRequest, User>()
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore());
        }
    }
}
