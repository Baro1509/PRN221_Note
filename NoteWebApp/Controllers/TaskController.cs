using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteWebApp.Request;
using NoteWebApp.Response;
using Repository;

namespace NoteWebApp.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    [EnableCors]
    public class TaskController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly TaskItemRepository _taskItemRepository;

        public TaskController(TaskRepository taskRepository, IMapper mapper, TaskItemRepository taskItemRepository)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _taskItemRepository = taskItemRepository;
        }

        //[HttpGet]
        //[ActionName("GetAllTasks")]
        //public IActionResult GetAllTasks([FromHeader] TaskDateRequest taskreq) {
        //    var user = HttpContext.User;
        //    if (user == null) {
        //        return Unauthorized();
        //    }

        //    if (!user.HasClaim(p => p.Type == "UserId")) {
        //        return Unauthorized();
        //    }

        //    if (DateTime.Compare(taskreq.dateFrom, taskreq.dateTo) > 0) {
        //        return BadRequest(new {
        //            message = "Datefrom should be before dateto"
        //        });
        //    }
        //    var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
        //    var tasks = _taskRepository.GetAll()
        //        .Where(p => p.UserId == userid &&
        //            DateTime.Compare(p.CreatedAt, taskreq.dateTo) <= 0 &&
        //            DateTime.Compare(taskreq.dateFrom, p.CreatedAt) <= 0 &&
        //            p.IsDelete == false)
        //        .Include(p => p.TaskItems.Where(o => o.IsDelete == false))
        //        .Select(p => _mapper.Map<TaskWithTaskItemResponse>(p));
        //    if (tasks == null) {
        //        return Ok(new {
        //            message = "You do not have any tasks for this date range"
        //        });
        //    }

        //    return Ok(new {taskList = tasks});
        //}

        [HttpGet]
        [ActionName("GetAllTasksWithSort")]
        public IActionResult GetAllTasks([FromQuery] TaskDateRequest taskreq, int orderBy, bool? isAsc)
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

            if (taskreq.dateFrom > taskreq.dateTo)
            {
                return BadRequest(new
                {
                    message = "Datefrom should be before dateto"
                });
            }
            var dateFrom = DateTimeOffset.FromUnixTimeMilliseconds(taskreq.dateFrom).DateTime;
            var dateTo = DateTimeOffset.FromUnixTimeMilliseconds(taskreq.dateTo).DateTime;

            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            var tasks = _taskRepository.GetAll()
                .Where(p => p.UserId == userid &&
                    DateTime.Compare(p.CreatedAt, dateTo) <= 0 &&
                    DateTime.Compare(dateFrom, p.CreatedAt) <= 0 &&
                    p.IsDelete == false)
                .Include(p => p.TaskItems.Where(o => o.IsDelete == false))
                .Select(p => _mapper.Map<TaskWithTaskItemResponse>(p)).ToList();
            if (tasks == null || tasks.Count == 0)
            {
                return Ok(new
                {
                    message = "You do not have any tasks for this date range"
                });
            }
            if (!isAsc == null)
            {
                if (orderBy != 0 && orderBy != 1)
                {
                    return BadRequest(new
                    {
                        message = "Sort param error"
                    });
                }
                if (isAsc == false)
                {
                    if (orderBy == 1)
                    {
                        tasks = tasks.OrderByDescending(p => p.Progress).ToList();
                    }
                    else
                    {
                        tasks = tasks.OrderByDescending(p => p.Priority).ToList();
                    }
                }
                else
                {
                    if (orderBy == 1)
                    {
                        tasks = tasks.OrderBy(p => p.Progress).ToList();
                    }
                    else
                    {
                        tasks = tasks.OrderBy(p => p.Priority).ToList();
                    }
                }
            }
            return Ok(new { taskList = tasks });
        }

        [HttpGet]
        [ActionName("GetOneTask")]
        [Route("/api/tasks/{taskId:guid}")]
        public IActionResult GetTask(Guid taskId)
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
            var task = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == taskId && p.IsDelete == false)
                .Include(p => p.TaskItems.Where(o => o.IsDelete == false))
                .Select(p => _mapper.Map<TaskWithTaskItemResponse>(p))
                .FirstOrDefault();
            if (task == null)
            {
                return Ok(new
                {
                    message = "Task not found"
                });
            }
            return Ok(new { task = task });
        }

        [HttpPost]
        public IActionResult Create([FromBody] TaskRequest task)
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

            if (!task.validation())
            {
                return BadRequest(new
                {
                    message = "The request is not in the correct format"
                });
            }

            var userid = Guid.Parse(user.Claims.FirstOrDefault(p => p.Type == "UserId").Value);
            if (userid != task.UserId)
            {
                return Unauthorized(new
                {
                    message = "You are now allowed to create task for this user"
                });
            }

            var taskInDB = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == task.Id && p.IsDelete == false)
                .FirstOrDefault();
            if (taskInDB != null)
            {
                return BadRequest(new
                {
                    message = "Task already exist"
                });
            }

            DateTime now = DateTime.Now;
            task.CreatedAt = now;
            task.UpdatedAt = now;
            task.StartDate = now;
            task.IsDelete = false;
            var taskInsert = _mapper.Map<Repository.Models.Task>(task);
            _taskRepository.Create(taskInsert);
            taskInDB = _taskRepository.GetAll().Where(p => p.CreatedAt == now && p.UserId == taskInsert.UserId).FirstOrDefault();
            if (taskInDB == null)
            {
                return NotFound(new
                {
                    message = "Something went wrong"
                });
            }
            return Ok(new
            {
                message = "Task created successfully",
                taskid = taskInDB.Id
            });
        }

        [HttpPut]
        public IActionResult Update([FromBody] TaskRequest task)
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
            if (userid != task.UserId)
            {
                return Unauthorized(new
                {
                    message = "You are now allowed to update task for this user"
                });
            }

            DateTime now = DateTime.Now;
            if (!task.validation())
            {
                return BadRequest(new
                {
                    message = "The request is not in the correct format"
                });
            }

            var taskInDB = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == task.Id && p.IsDelete == false)
                .FirstOrDefault();
            if (taskInDB == null)
            {
                return NotFound(new
                {
                    message = "The task you are trying to update is not available"
                });
            }

            taskInDB.Title = task.Title;
            taskInDB.Progress = task.Progress;
            taskInDB.Description = task.Description;
            taskInDB.StartDate = task.StartDate;
            taskInDB.UpdatedAt = now;
            taskInDB.Priority = task.Priority;
            _taskRepository.Update(taskInDB);
            return Ok(new
            {
                message = "Task updated successfully"
            });
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] Guid id)
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
            var taskInDB = _taskRepository.GetAll()
                .Where(p => p.UserId == userid && p.Id == id && p.IsDelete == false)
                .Include(p => p.TaskItems.Where(o => o.IsDelete == false))
                .FirstOrDefault();
            if (taskInDB == null)
            {
                return NotFound(new
                {
                    message = "The task you are trying to delete is not available"
                });
            }
            if (userid != taskInDB.UserId)
            {
                return Unauthorized(new
                {
                    message = "You are now allowed to delete task for this user"
                });
            }

            taskInDB.TaskItems.ToList().ForEach(p =>
            {
                p.IsDelete = true;
                p.UpdatedAt = DateTime.Now;
                _taskItemRepository.Update(p);
            });
            taskInDB.IsDelete = true;
            taskInDB.UpdatedAt = DateTime.Now;
            _taskRepository.Update(taskInDB);
            return Ok(new
            {
                message = "Task deleted successfully",
                taskid = taskInDB.Id
            });
        }
    }
}
