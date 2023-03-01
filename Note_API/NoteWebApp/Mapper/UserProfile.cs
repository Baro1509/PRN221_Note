using AutoMapper;
using NoteWebApp.Response;
using Repository.Models;

namespace NoteWebApp.Mapper {
    public class UserProfile : Profile {
        public UserProfile() {
            CreateMap<User, UserResponse>();
        }
    }
}
