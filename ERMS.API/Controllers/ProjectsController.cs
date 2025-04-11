// File: Controllers/ProjectsController.cs
using ERMS.API.Interfaces;
using ERMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ERMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectData _data;

        public ProjectsController(IProjectData data)
        {
            _data = data;
        }

        [HttpGet("/api/Projects")]
        public ActionResult<List<Project>> GetAll() => _data.GetAllProjects();

        [HttpGet("/api/Project/{id}", Name = "GetProject")]
        public ActionResult<Project> GetById(int? id)
        {
            var item = _data.GetProjectById(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("/api/createProject", Name = "CreateProject")]
        public ActionResult<Project> CreateProject([FromBody] Project project)
        {
            if (project == null) return BadRequest("Project is null");
            _data.CreateProject(project);
            return project;
        }

        [HttpPut("/api/editProject/{id}", Name = "EditProject")]
        public ActionResult<Project> EditProject(int? id, [FromBody] Project project)
        {
            var updated = _data.UpdateProject(id, project);
            if (updated == null || id != project.Id)
                return BadRequest("Project does not exist or ID mismatch");
            return updated;
        }

        [HttpDelete("/api/deleteProject/{id}", Name = "DeleteProject")]
        public ActionResult<Project> DeleteProject(int? id)
        {
            var item = _data.GetProjectById(id);
            if (item != null && _data.DeleteProject(id))
                return Ok(item);
            return NotFound();
        }
    }
}
