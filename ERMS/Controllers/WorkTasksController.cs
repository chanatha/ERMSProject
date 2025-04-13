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
    [Authorize(Roles = "Admin,Manager")]
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

        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks
                .FromSqlRaw("EXEC GetAllTasks")
                .Include(t => t.Project)
                .Include(t => t.Employee)
                .ToListAsync();

            return View(tasks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.Tasks
                .FromSqlRaw("EXEC GetTaskById @p0", id)
                .Include(t => t.Project)
                .Include(t => t.Employee)
                .FirstOrDefaultAsync();

            if (task == null) return NotFound();

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
                    "EXEC CreateTask @p0, @p1, @p2, @p3, @p4, @p5, @p6",
                    parameters: [
                        task.Title,
                        task.Description,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.ProjectId,
                        task.EmployeeId
                    ]);

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync();
            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks
                .FromSqlRaw("EXEC GetTaskById @p0", id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

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
                    "EXEC UpdateTask @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                    parameters: [
                        task.Id,
                        task.Title,
                        task.Description,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.ProjectId,
                        task.EmployeeId
                    ]);

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownsAsync();
            return View(task);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks
                .FromSqlRaw("EXEC GetTaskById @p0", id)
                .Include(t => t.Project)
                .Include(t => t.Employee)
                .FirstOrDefaultAsync();

            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteTask @p0", id);
            return RedirectToAction(nameof(Index));
        }
    }
}