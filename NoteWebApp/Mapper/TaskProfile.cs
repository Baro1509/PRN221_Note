using AutoMapper;
using NoteWebApp.Request;
using NoteWebApp.Response;

namespace NoteWebApp.Mapper {
    public class TaskProfile : Profile{
        public TaskProfile() {
            CreateMap<Repository.Models.Task, TaskResponse>();
            CreateMap<Repository.Models.Task, TaskWithTaskItemResponse>();
            CreateMap<Repository.Models.TaskItem, TaskItemResponse>();

            CreateMap<TaskRequest, Repository.Models.Task>()
                .ForMember(dest => dest.TaskItems, opt => opt.Ignore());
        }
    }
}
