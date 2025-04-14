using ERMS.Data;
using ERMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class WorkTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WorkTasksController> _logger;

        public WorkTasksController(ApplicationDbContext context, ILogger<WorkTasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewData["Projects"] = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name");
            ViewData["Employees"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName");
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var tasks = _context.Tasks
                    .FromSqlRaw("EXEC GetAllWorkTasks")
                    .AsNoTracking()
                    .ToList()
                    .Select(t =>
                    {
                        t.Project = _context.Projects.Find(t.ProjectId);
                        t.Employee = t.EmployeeId.HasValue ? _context.Employees.Find(t.EmployeeId.Value) : null;
                        return t;
                    })
                    .ToList();

                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving work tasks.");
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var taskList = await _context.Tasks
                    .FromSqlRaw("EXEC GetWorkTaskById @p0", id)
                    .AsNoTracking()
                    .ToListAsync();

                var task = taskList.FirstOrDefault();
                if (task == null) return NotFound();

                task.Project = await _context.Projects.FindAsync(task.ProjectId);
                task.Employee = task.EmployeeId.HasValue ? await _context.Employees.FindAsync(task.EmployeeId.Value) : null;

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task details for ID {TaskId}.", id);
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkTask task)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC InsertWorkTask @p0, @p1, @p2, @p3, @p4, @p5, @p6",
                        task.Title,
                        task.Description ?? string.Empty,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.ProjectId,
                        task.EmployeeId
                    );

                    _logger.LogInformation("Task '{TaskTitle}' created successfully.", task.Title);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating task.");
                    return View("Error");
                }
            }

            await PopulateDropdownsAsync();
            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var taskList = await _context.Tasks
                    .FromSqlRaw("EXEC GetWorkTaskById @p0", id)
                    .AsNoTracking()
                    .ToListAsync();

                var task = taskList.FirstOrDefault();
                if (task == null) return NotFound();

                await PopulateDropdownsAsync();
                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading task for editing with ID {TaskId}.", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkTask task)
        {
            if (id != task.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC UpdateWorkTask @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                        task.Id,
                        task.Title,
                        task.Description ?? string.Empty,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.ProjectId,
                        task.EmployeeId
                    );

                    _logger.LogInformation("Task ID {TaskId} updated successfully.", id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating task ID {TaskId}.", id);
                    return View("Error");
                }
            }

            await PopulateDropdownsAsync();
            return View(task);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var taskList = await _context.Tasks
                    .FromSqlRaw("EXEC GetWorkTaskById @p0", id)
                    .AsNoTracking()
                    .ToListAsync();

                var task = taskList.FirstOrDefault();
                if (task == null) return NotFound();

                task.Project = await _context.Projects.FindAsync(task.ProjectId);
                task.Employee = task.EmployeeId.HasValue ? await _context.Employees.FindAsync(task.EmployeeId.Value) : null;

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading task for deletion with ID {TaskId}.", id);
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC DeleteWorkTask @p0", id);
                _logger.LogInformation("Task ID {TaskId} deleted successfully.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task ID {TaskId}.", id);
                return View("Error");
            }
        }
    }
}
