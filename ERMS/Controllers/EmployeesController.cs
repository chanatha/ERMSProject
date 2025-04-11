using Microsoft.AspNetCore.Mvc;
using ERMS.Models;
using ERMS.Services;

namespace ERMS.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeApiService _service;

        public EmployeesController(EmployeeApiService service)
        {
            _service = service;
        }

        private string GetToken()
        {
            // In production, retrieve from session or secure store
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJleHAiOjE3NDM4NzE5Mjd9.DHS5Fs5dCk4jTy8f_exO0Q0vnUjQSI7tiod3_ZOVT0g"; // Replace with actual token
        }

        public async Task<IActionResult> Index()
        {
            var token = GetToken();
            var employees = await _service.GetAllAsync(token);
            return View(employees);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var token = GetToken();
            var emp = await _service.GetByIdAsync(id.Value, token);
            return View(emp);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(employee, GetToken());
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var token = GetToken();
            var emp = await _service.GetByIdAsync(id.Value, token);
            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(employee, GetToken());
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var emp = await _service.GetByIdAsync(id.Value, GetToken());
            return View(emp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id, GetToken());
            return RedirectToAction(nameof(Index));
        }
    }
}
