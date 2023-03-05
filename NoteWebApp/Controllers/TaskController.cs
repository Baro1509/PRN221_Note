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

        public TaskController(TaskRepository taskRepository, IMapper mapper) {
            _taskRepository = taskRepository;
            _mapper = mapper;
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

            var header = Request.Headers;
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
                .Where(p => p.UserId == userid && DateTime.Compare(p.CreatedAt, taskreq.DateTo) <= 0 && DateTime.Compare(taskreq.DateFrom, p.CreatedAt) <= 0)
                .Include(p=>p.TaskItems)
                .Select(p => _mapper.Map<TaskWithTaskItemResponse>(p));
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
            var task = _taskRepository.GetAll().Where(p => p.UserId == userid && p.Id == taskId).Include(p => p.TaskItems).Select(p => _mapper.Map<TaskWithTaskItemResponse>(p)).FirstOrDefault();
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

            var taskInDB = _taskRepository.GetAll().Where(p => p.UserId == userid && p.Id == task.Id).FirstOrDefault();
            if (taskInDB != null) {
                 
            }

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

            var taskInDB = _taskRepository.GetAll().Where(p => p.UserId == userid && p.Id == task.Id).FirstOrDefault();
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
            var taskInDB = _taskRepository.GetAll().Where(p => p.UserId == userid && p.Id == id).FirstOrDefault();
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
            _taskRepository.Delete(taskInDB);
            return Ok(new {
                message = "Task deleted successfully"
            });
        }
    }
}
