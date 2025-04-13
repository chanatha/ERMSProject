using ERMS.Data;
using ERMS.Models;
using ERMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .FromSqlRaw("EXEC GetAllProjects")
                .ToListAsync();
            return View(projects);
        }
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int id)
        {
            var param = new SqlParameter("@Id", id);
            var project = await _context.Projects
                .FromSqlRaw("EXEC GetProjectById @Id", param)
                .FirstOrDefaultAsync();

            if (project == null) return NotFound();
            return View(project);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                var parameters = new[]
                {
                    new SqlParameter("@Name", project.Name ?? ""),
                    new SqlParameter("@StartDate", project.StartDate),
                    new SqlParameter("@EndDate", (object?)project.EndDate ?? DBNull.Value)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC CreateProject @Name, @StartDate, @EndDate", parameters);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var param = new SqlParameter("@Id", id);
            var project = await _context.Projects
                .FromSqlRaw("EXEC GetProjectById @Id", param)
                .FirstOrDefaultAsync();

            if (project == null) return NotFound();
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Name", project.Name ?? ""),
                    new SqlParameter("@StartDate", project.StartDate),
                    new SqlParameter("@EndDate", (object?)project.EndDate ?? DBNull.Value)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateProject @Id, @Name, @StartDate, @EndDate", parameters);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var param = new SqlParameter("@Id", id);
            var project = await _context.Projects
                .FromSqlRaw("EXEC GetProjectById @Id", param)
                .FirstOrDefaultAsync();

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var param = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteProject @Id", param);
            return RedirectToAction(nameof(Index));
        }
    }
}
