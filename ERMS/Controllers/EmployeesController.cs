using Microsoft.AspNetCore.Mvc;
using ERMS.Models;
using ERMS.Services;
using ERMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .FromSqlRaw("EXEC GetAllEmployees")
                .ToListAsync();
            return View(employees);
        }
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = _context.Employees
                .FromSqlRaw("EXEC GetEmployeeById @p0", id)
                .AsEnumerable()
                .FirstOrDefault();

            if (employee == null) return NotFound();
            return View(employee);
        }



        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC InsertEmployee @p0, @p1, @p2, @p3, @p4, @p5",
                    employee.FirstName, employee.LastName,employee.Email,
                    employee.Department, employee.Position, employee.HireDate);

                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = _context.Employees
                .FromSqlRaw("EXEC GetEmployeeById @p0", id)
                .AsEnumerable()
                .FirstOrDefault();

            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateEmployee @p0, @p1, @p2, @p3, @p4, @p5",
                    employee.Id, employee.FirstName, employee.LastName,employee.Email,
                    employee.Position, employee.HireDate);

                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = _context.Employees
                .FromSqlRaw("EXEC GetEmployeeById @p0", id)
                .AsEnumerable()
                .FirstOrDefault();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC DeleteEmployee @p0", id);
            return RedirectToAction(nameof(Index));
        }
    }
}
