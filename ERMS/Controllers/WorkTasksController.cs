using ERMS.Data;
using ERMS.Models;
using ERMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Controllers
{
    [Authorize(Roles = "Admin,Manager")] // Default role restriction
    public class WorkTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewData["Projects"] = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name");
            ViewData["Employees"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName");
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            var tasks = _context.Tasks
                .FromSqlRaw("EXEC GetAllWorkTasks")
                .AsNoTracking()
                .ToList() // Forces in-memory composition
                .Select(t =>
                {
                    t.Project = _context.Projects.Find(t.ProjectId);
                    t.Employee = t.EmployeeId.HasValue ? _context.Employees.Find(t.EmployeeId.Value) : null;
                    return t;
                })
                .ToList();

            return View(tasks);
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int id)
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

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync();
            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkTask task)
        {
            if (id != task.Id) return NotFound();

            if (ModelState.IsValid)
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

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync();
            return View(task);
        }

        public async Task<IActionResult> Delete(int id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteWorkTask @p0", id);
            return RedirectToAction(nameof(Index));
        }
    }
}