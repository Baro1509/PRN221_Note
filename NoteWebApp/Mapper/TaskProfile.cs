using AutoMapper;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository.Models;

namespace NoteWebApp.Mapper {
    public class TaskProfile : Profile{
        public TaskProfile() {
            CreateMap<Repository.Models.Task, TaskResponse>();
            CreateMap<Repository.Models.Task, TaskWithTaskItemResponse>();
            CreateMap<TaskItem, TaskItemResponse>();

            CreateMap<TaskRequest, Repository.Models.Task>()
                .ForMember(dest => dest.TaskItems, opt => opt.Ignore());
            CreateMap<TaskItemRequest, TaskItem>();
        }
    }
}
