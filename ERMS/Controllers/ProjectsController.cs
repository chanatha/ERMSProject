using ERMS.Data;
using ERMS.Models;
using ERMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ERMS.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProjectsController> _logger;


        public ProjectsController(ApplicationDbContext context, ILogger<ProjectsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var projects = await _context.Projects
                    .FromSqlRaw("EXEC GetAllProjects")
                    .ToListAsync();
                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project list.");
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var param = new SqlParameter("@Id", id);
                var project = _context.Projects
                    .FromSqlRaw("EXEC GetProjectById @Id", param)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (project == null)
                {
                    _logger.LogWarning("Project with ID {ProjectId} not found.", id);
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project details for ID {ProjectId}.", id);
                return View("Error");
            }
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new[]
                    {
                    new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = project.Name ?? (object)DBNull.Value },
                    new SqlParameter("@Description", SqlDbType.NVarChar) { Value = project.Description ?? (object)DBNull.Value },
                    new SqlParameter("@StartDate", SqlDbType.Date) { Value = project.StartDate },
                    new SqlParameter("@EndDate", SqlDbType.Date) { Value = project.EndDate.HasValue ? (object)project.EndDate.Value : DBNull.Value }
                };

                    await _context.Database.ExecuteSqlRawAsync("EXEC InsertProject @Name, @Description, @StartDate, @EndDate", parameters);
                    _logger.LogInformation("Project '{ProjectName}' created successfully.", project.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating project '{ProjectName}'.", project.Name);
                    return View("Error");
                }
            }
            return View(project);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var param = new SqlParameter("@Id", id);
                var project = _context.Projects
                    .FromSqlRaw("EXEC GetProjectById @Id", param)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (project == null)
                {
                    _logger.LogWarning("Project with ID {ProjectId} not found for editing.", id);
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading project with ID {ProjectId} for editing.", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var parameters = new[]
                    {
                    new SqlParameter("@Id", SqlDbType.Int) { Value = project.Id },
                    new SqlParameter("@Name", SqlDbType.NVarChar, 100) { Value = project.Name ?? (object)DBNull.Value },
                    new SqlParameter("@Description", SqlDbType.NVarChar) { Value = project.Description ?? (object)DBNull.Value },
                    new SqlParameter("@StartDate", SqlDbType.Date) { Value = project.StartDate },
                    new SqlParameter("@EndDate", SqlDbType.Date) { Value = project.EndDate.HasValue ? (object)project.EndDate.Value : DBNull.Value }
                };

                    await _context.Database.ExecuteSqlRawAsync("EXEC UpdateProject @Id, @Name, @Description, @StartDate, @EndDate", parameters);
                    _logger.LogInformation("Project ID {ProjectId} updated successfully.", project.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating project ID {ProjectId}.", project.Id);
                    return View("Error");
                }
            }
            return View(project);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var param = new SqlParameter("@Id", id);
                var project = _context.Projects
                    .FromSqlRaw("EXEC GetProjectById @Id", param)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (project == null)
                {
                    _logger.LogWarning("Project with ID {ProjectId} not found for deletion.", id);
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading project with ID {ProjectId} for deletion.", id);
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var param = new SqlParameter("@Id", id);
                await _context.Database.ExecuteSqlRawAsync("EXEC DeleteProject @Id", param);
                _logger.LogInformation("Project ID {ProjectId} deleted successfully.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project ID {ProjectId}.", id);
                return View("Error");
            }
        }
    }
}