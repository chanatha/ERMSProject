using Microsoft.AspNetCore.Mvc;
using ERMS.Models;
using ERMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERMS.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _context.Employees
                    .FromSqlRaw("EXEC GetAllEmployees")
                    .ToListAsync();
                return View(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees.");
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var employee = _context.Employees
                    .FromSqlRaw("EXEC GetEmployeeById @p0", id)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (employee == null) return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee details for ID {EmployeeId}.", id);
                return View("Error");
            }
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC InsertEmployee @p0, @p1, @p2, @p3, @p4, @p5",
                        employee.FirstName, employee.LastName, employee.Email,
                        employee.Department, employee.Position, employee.HireDate);

                    _logger.LogInformation("Employee '{FirstName} {LastName}' created successfully.", employee.FirstName, employee.LastName);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee.");
                    return View("Error");
                }
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var employee = _context.Employees
                    .FromSqlRaw("EXEC GetEmployeeById @p0", id)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (employee == null) return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee for editing with ID {EmployeeId}.", id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("EXEC UpdateEmployee @p0, @p1, @p2, @p3, @p4, @p5, @p6",
                        employee.Id, employee.FirstName, employee.LastName, employee.Email,
                        employee.Department, employee.Position, employee.HireDate);

                    _logger.LogInformation("Employee ID {EmployeeId} updated successfully.", id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee ID {EmployeeId}.", id);
                    return View("Error");
                }
            }

            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var employee = _context.Employees
                    .FromSqlRaw("EXEC GetEmployeeById @p0", id)
                    .AsEnumerable()
                    .FirstOrDefault();

                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employee for deletion with ID {EmployeeId}.", id);
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC DeleteEmployee @p0", id);
                _logger.LogInformation("Employee ID {EmployeeId} deleted successfully.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee ID {EmployeeId}.", id);
                return View("Error");
            }
        }
    }
}