using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository;
using Repository.Models;
using System.Threading.Tasks;

namespace NoteWebApp.Controllers {
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase {
        private readonly TaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly TaskItemRepository _taskItemRepository;

        private readonly byte LOW_PRIORITY = 2;
        private readonly byte MEDIUM_PRIORITY = 1;
        private readonly byte HIGH_PRIORITY = 0;

        private readonly byte PLAN_PROGRESS = 0;
        private readonly byte PROGRESS_PROGRESS = 1;
        private readonly byte REVIEW_PROGRESS = 2;
        private readonly byte DONE_PROGRESS = 3;

        public TaskController(TaskRepository taskRepository, IMapper mapper, TaskItemRepository taskItemRepository) {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _taskItemRepository = taskItemRepository;
        }

        [HttpGet]
        [ActionName("GetAllTasks")]
        public IActionResult GetAllTasks([FromHeader] TaskDateRequest taskreq) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }

            if (taskreq == null) {
                return BadRequest();
            }

            if (DateTime.Compare(taskreq.DateFrom, taskreq.DateTo) > 0) {
                return BadRequest(new {
                    message = "Datefrom should be before dateto"
                });
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var tasks = _taskRepository.GetAll()
                .Where(p => p.UserId == userid &&
                    DateTime.Compare(p.CreatedAt, taskreq.DateTo) <= 0 &&
                    DateTime.Compare(taskreq.DateFrom, p.CreatedAt) <= 0 &&
                    p.IsDelete == false)
                .Include(p => p.TaskItems.Where(o => o.IsDelete == false))
                .Select(p => _mapper.Map<TaskWithTaskItemResponse>(p));
            if (tasks == null) {
                return Ok(new {
                    message = "You do not have any tasks for this date range"
                });
            }
            return Ok(new {taskList = tasks});
        }

        [HttpGet]
        [ActionName("GetOneTask")]
        [Route("/api/tasks/{taskId:guid}")]
        public IActionResult GetTask(Guid taskId) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }
            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var task = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == taskId && p.IsDelete == false)
                .Include(p => p.TaskItems.Where(o => o.IsDelete == false))
                .Select(p => _mapper.Map<TaskWithTaskItemResponse>(p))
                .FirstOrDefault();
            if (task == null) {
                return Ok(new {
                    message = "Task not found"
                });
            }
            return Ok(new {task = task});
        }

        [HttpPost]
        public IActionResult Create([FromBody]TaskRequest task) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }

            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            if (userid != task.UserId) {
                return Unauthorized(new {
                    message = "You are now allowed to create task for this user"
                });
            }

            var taskInDB = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == task.Id && p.IsDelete == false)
                .FirstOrDefault();
            if (taskInDB != null) {
                return BadRequest(new {
                    message = "Task already exist"
                });
            }

            task.Progress = HIGH_PRIORITY;
            task.Progress = PLAN_PROGRESS;
            task.CreatedAt = DateTime.Now;
            task.UpdatedAt = DateTime.Now;
            _taskRepository.Create(_mapper.Map<Repository.Models.Task>(task));
            return Ok(new {
                message = "Task created successfully"
            });
        }

        [HttpPut]
        public IActionResult Update([FromBody] TaskRequest task) {
            var user = HttpContext.User;
            if (user == null) {
                return Unauthorized();
            }

            if (!user.HasClaim(p => p.Type == "UserId")) {
                return Unauthorized();
            }

            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            if (userid != task.UserId) {
                return Unauthorized(new {
                    message = "You are now allowed to update task for this user"
                });
            }

            var taskInDB = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == task.Id && p.IsDelete == false)
                .FirstOrDefault();
            if (taskInDB == null) {
                return NotFound(new {
                    message = "The task you are trying to update is not available"
                });
            }

            taskInDB.Title = task.Title;
            taskInDB.Description = task.Description;
            taskInDB.UpdatedAt = DateTime.Now;
            taskInDB.Priority = task.Priority;
            _taskRepository.Update(taskInDB);
            return Ok(new {
                message = "Task updated successfully"
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
            var taskInDB = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == id && p.IsDelete == false)
                .Include(p => p.TaskItems.Where(o => o.IsDelete == false))
                .FirstOrDefault();
            if (taskInDB == null) {
                return NotFound(new {
                    message = "The task you are trying to delete is not available"
                });
            }
            if (userid != taskInDB.UserId) {
                return Unauthorized(new {
                    message = "You are now allowed to delete task for this user"
                });
            }

            taskInDB.TaskItems.ToList().ForEach(p => {
                p.IsDelete = true;
                p.UpdatedAt = DateTime.Now;
                _taskItemRepository.Update(p);
            });
            taskInDB.IsDelete = true;
            taskInDB.UpdatedAt = DateTime.Now;
            _taskRepository.Update(taskInDB);
            return Ok(new {
                message = "Task deleted successfully"
            });
        }
    }
}
