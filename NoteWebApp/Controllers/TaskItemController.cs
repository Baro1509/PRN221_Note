using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository;
using Repository.Models;

namespace NoteWebApp.Controllers {
    [Route("api/tasks/taskitems")]
    [ApiController]
    public class TaskItemController : ControllerBase {
        private readonly TaskItemRepository _taskItemRepository;
        private readonly TaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskItemController(TaskItemRepository taskItemRepository, IMapper mapper, TaskRepository taskRepository) {
            _taskItemRepository = taskItemRepository;
            _mapper = mapper;
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public IActionResult GetAllTaskItems([FromHeader] Guid taskid) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var task = _taskRepository.GetAll().Where(p => p.Id == taskid && p.UserId == userid).FirstOrDefault();
            if (task == null) {
                return NotFound(new {
                    message = "Task not found"
                });
            }
            var taskItems = _taskItemRepository.GetAll().Where(p => p.TaskId == taskid).Select(p => _mapper.Map<TaskItemResponse>(p));
            return Ok(new {
                taskItems = taskItems
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] TaskItemRequest taskItemRequest) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }
            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var task = _taskRepository.GetAll().Where(p => p.Id == taskItemRequest.TaskId && p.UserId == userid).FirstOrDefault();
            if (task == null) {
                return NotFound(new {
                    message = "Task not found"
                });
            }

            var taskItem = _taskItemRepository.GetAll().Where(p => p.Id == taskItemRequest.Id && p.TaskId == taskItemRequest.TaskId).FirstOrDefault();
            if (taskItem != null) {
                return BadRequest(new {
                    message = "Task item already exist"
                });
            }

            taskItemRequest.IsCompleted = false;
            taskItemRequest.CreatedAt = DateTime.Now;
            taskItemRequest.UpdatedAt = DateTime.Now;
            _taskItemRepository.Create(_mapper.Map<TaskItem>(taskItemRequest));
            return Ok(new {
                message = "Task item created successfully"
            });
        }

        [HttpPut]
        //TODO: update a task item will update the date of the task
        public IActionResult Update([FromBody] TaskItemRequest taskItemRequest) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }
            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var task = _taskRepository.GetAll().Where(p => p.Id == taskItemRequest.TaskId && p.UserId == userid).FirstOrDefault();
            if (task == null) {
                return NotFound(new {
                    message = "Task not found"
                });
            }

            var taskItem = _taskItemRepository.GetAll().Where(p => p.Id == taskItemRequest.Id && p.TaskId == taskItemRequest.TaskId).FirstOrDefault();
            if (taskItem == null) {
                return NotFound(new {
                    message = "Task item not found"
                });
            }

            taskItem.Title = taskItemRequest.Title;
            taskItem.Description = taskItemRequest.Description;
            taskItem.IsCompleted = taskItemRequest.IsCompleted;
            taskItem.Deadline = taskItemRequest.Deadline;
            taskItem.Priority = taskItemRequest.Priority;
            taskItem.UpdatedAt = DateTime.Now;
            _taskItemRepository.Update(taskItem);
            return Ok(new {
                message = "Task item updated successfully"
            });
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] Guid id) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }
            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);

            var taskItem = _taskItemRepository.GetAll().Where(p => p.Id == id).FirstOrDefault();
            if (taskItem == null) {
                return NotFound(new {
                    message = "Task item not found"
                });
            }

            var task = _taskRepository.GetAll().Where(p => p.Id == taskItem.TaskId).FirstOrDefault();
            if (task == null) {
                return NotFound(new {
                    message = "Task not found, denying delete"
                });
            }

            if (task.UserId != userid) {
                return Unauthorized(new {
                    message = "You are not allowed to delete this task item"
                });
            }

            _taskItemRepository.Delete(taskItem);
            return Ok(new {
                message = "Task item deleted successfully"
            });
        }
    }
}
