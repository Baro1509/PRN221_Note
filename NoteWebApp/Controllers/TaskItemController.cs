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

        private readonly byte LOW_PRIORITY = 2;
        private readonly byte MEDIUM_PRIORITY = 1;
        private readonly byte HIGH_PRIORITY = 0;

        private readonly byte PLAN_PROGRESS = 0;
        private readonly byte PROGRESS_PROGRESS = 1;
        private readonly byte REVIEW_PROGRESS = 2;
        private readonly byte DONE_PROGRESS = 3;

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
            var task = _taskRepository.GetAll()
                .Where(p => p.Id == taskid && p.UserId == userid && p.IsDelete == false)
                .FirstOrDefault();
            if (task == null) {
                return NotFound(new {
                    message = "Task not found"
                });
            }
            var taskItems = _taskItemRepository.GetAll().Where(p => p.TaskId == taskid && p.IsDelete == false).Select(p => _mapper.Map<TaskItemResponse>(p));
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
            var task = _taskRepository.GetAll()
                .Where(p => p.Id == taskItemRequest.TaskId && p.UserId == userid && p.IsDelete == false)
                .FirstOrDefault();
            if (task == null) {
                return NotFound(new {
                    message = "Task not found"
                });
            }

            var taskItem = _taskItemRepository.GetAll()
                .Where(p => p.Id == taskItemRequest.Id && p.TaskId == taskItemRequest.TaskId && p.IsDelete == false)
                .FirstOrDefault();
            if (taskItem != null) {
                return BadRequest(new {
                    message = "Task item already exist"
                });
            }
            task.UpdatedAt = DateTime.Now;
            _taskRepository.Update(task);

            taskItemRequest.IsDelete = false;
            taskItemRequest.Priority = HIGH_PRIORITY;
            taskItemRequest.Progress = PLAN_PROGRESS;
            taskItemRequest.CreatedAt = DateTime.Now;
            taskItemRequest.StartDate = DateTime.Now;
            taskItemRequest.UpdatedAt = DateTime.Now;
            _taskItemRepository.Create(_mapper.Map<TaskItem>(taskItemRequest));
            return Ok(new {
                message = "Task item created successfully"
            });
        }

        [HttpPut]
        public IActionResult Update([FromBody] TaskItemRequest taskItemRequest) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }
            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var task = _taskRepository.GetAll()
                .Where(p => p.Id == taskItemRequest.TaskId && p.UserId == userid && p.IsDelete == false)
                .FirstOrDefault();
            if (task == null) {
                return NotFound(new {
                    message = "Task not found"
                });
            }

            var taskItem = _taskItemRepository.GetAll()
                .Where(p => p.Id == taskItemRequest.Id && p.TaskId == taskItemRequest.TaskId && p.IsDelete == false)
                .FirstOrDefault();
            if (taskItem == null) {
                return NotFound(new {
                    message = "Task item not found"
                });
            }
            task.UpdatedAt = DateTime.Now;
            _taskRepository.Update(task);

            taskItem.Title = taskItemRequest.Title;
            taskItem.Description = taskItemRequest.Description;
            taskItem.Progress = taskItemRequest.Progress;
            taskItem.Deadline = taskItemRequest.Deadline;
            taskItem.Priority = taskItemRequest.Priority;
            taskItem.StartDate = taskItemRequest.StartDate;
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

            var taskItem = _taskItemRepository.GetAll().Where(p => p.Id == id && p.IsDelete == false).FirstOrDefault();
            if (taskItem == null) {
                return NotFound(new {
                    message = "Task item not found"
                });
            }

            var task = _taskRepository.GetAll().Where(p => p.Id == taskItem.TaskId && p.IsDelete == false).FirstOrDefault();
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
            task.UpdatedAt = DateTime.Now;
            _taskRepository.Update(task);

            _taskItemRepository.Delete(taskItem);
            return Ok(new {
                message = "Task item deleted successfully"
            });
        }
    }
}
