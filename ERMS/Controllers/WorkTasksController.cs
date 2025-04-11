using ERMS.Models;
using ERMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERMS.Controllers
{
    public class WorkTasksController : Controller
    {
        private readonly WorkTaskApiService _taskApiService;
        private readonly ProjectApiService _projectApiService;
        private readonly EmployeeApiService _employeeApiService;

        public WorkTasksController(WorkTaskApiService taskApiService,
                                   ProjectApiService projectApiService,
                                   EmployeeApiService employeeApiService)
        {
            _taskApiService = taskApiService;
            _projectApiService = projectApiService;
            _employeeApiService = employeeApiService;
        }

        private string GetToken()
        {
            // In production, retrieve from session or secure store
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJleHAiOjE3NDM4NzE5Mjd9.DHS5Fs5dCk4jTy8f_exO0Q0vnUjQSI7tiod3_ZOVT0g"; // Replace with actual token
        }

        private async Task PopulateDropdowns()
        {
            var token = GetToken();

            var projects = await _projectApiService.GetAllAsync(token);
            ViewData["Projects"] = new SelectList(projects, "Id", "Name");

            var employees = await _employeeApiService.GetAllAsync(token);
            ViewData["Employees"] = new SelectList(employees, "Id", "FullName");
        }

        public async Task<IActionResult> Index()
        {
            var token = GetToken();

            var tasks = await _taskApiService.GetAllAsync(token);
            var projects = await _projectApiService.GetAllAsync(token);
            var employees = await _employeeApiService.GetAllAsync(token);

            var projectDict = projects.ToDictionary(p => p.Id);
            var employeeDict = employees.ToDictionary(e => e.Id);

            foreach (var task in tasks)
            {
                if (projectDict.TryGetValue(task.ProjectId, out var project))
                    task.Project = project;

                if (task.EmployeeId.HasValue && employeeDict.TryGetValue(task.EmployeeId.Value, out var employee))
                    task.Employee = employee;
            }

            return View(tasks);
        }



        public async Task<IActionResult> Details(int id)
        {
            var token = GetToken();
            var task = await _taskApiService.GetByIdAsync(id, token);
            if (task == null)
                return NotFound();

            task.Project = await _projectApiService.GetByIdAsync(task.ProjectId, token);

            if (task.EmployeeId.HasValue)
                task.Employee = await _employeeApiService.GetByIdAsync(task.EmployeeId.Value, token);

            return View(task);
        }


        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkTask task)
        {
            if (ModelState.IsValid)
            {
                await _taskApiService.CreateAsync(task, GetToken());
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns();
            return View(task);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskApiService.GetByIdAsync(id, GetToken());
            if (task == null)
                return NotFound();

            await PopulateDropdowns();
            return View(task);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkTask task)
        {
            if (ModelState.IsValid)
            {
                await _taskApiService.CreateAsync(task, GetToken());
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns();
            return View(task);
        }



        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskApiService.GetByIdAsync(id, GetToken());
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _taskApiService.DeleteAsync(id, GetToken());
            return RedirectToAction(nameof(Index));
        }
    }
}
