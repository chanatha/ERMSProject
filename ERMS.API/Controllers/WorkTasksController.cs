// File: Controllers/WorkTasksController.cs
using ERMS.API.Interfaces;
using ERMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ERMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTasksController : ControllerBase
    {
        private readonly IWorkTaskData _data;

        public WorkTasksController(IWorkTaskData data)
        {
            _data = data;
        }

        [HttpGet("/api/WorkTasks")]
        public ActionResult<List<WorkTask>> GetAll() => _data.GetAllTasks();

        [HttpGet("/api/WorkTask/{id}", Name = "GetWorkTask")]
        public ActionResult<WorkTask> GetById(int? id)
        {
            var item = _data.GetTaskById(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("/api/createWorkTask", Name = "CreateWorkTask")]
        public ActionResult<WorkTask> Create([FromBody] WorkTask task)
        {
            if (task == null) return BadRequest("Task is null");
            _data.CreateTask(task);
            return task;
        }

        [HttpPut("/api/editWorkTask/{id}", Name = "EditWorkTask")]
        public ActionResult<WorkTask> Edit(int? id, [FromBody] WorkTask task)
        {
            var updated = _data.UpdateTask(id, task);
            if (updated == null || id != task.Id)
                return BadRequest("Task does not exist or ID mismatch");
            return updated;
        }

        [HttpDelete("/api/deleteWorkTask/{id}", Name = "DeleteWorkTask")]
        public ActionResult<WorkTask> Delete(int? id)
        {
            var item = _data.GetTaskById(id);
            if (item != null && _data.DeleteTask(id))
                return Ok(item);
            return NotFound();
        }
    }
}
