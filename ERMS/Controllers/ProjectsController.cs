using ERMS.Models;
using ERMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ProjectApiService _apiService;

        public ProjectsController(ProjectApiService apiService)
        {
            _apiService = apiService;
        }

        private string GetToken()
        {
            // In production, retrieve from session or secure store
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJleHAiOjE3NDM4NzE5Mjd9.DHS5Fs5dCk4jTy8f_exO0Q0vnUjQSI7tiod3_ZOVT0g"; // Replace with actual token
        }


        public async Task<IActionResult> Index()
        {
            var projects = await _apiService.GetAllAsync(GetToken());
            return View(projects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await _apiService.GetByIdAsync(id, GetToken());
            return View(project);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateAsync(project, GetToken());
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _apiService.GetByIdAsync(id, GetToken());
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _apiService.UpdateAsync(project, GetToken());
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var project = await _apiService.GetByIdAsync(id, GetToken());
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteAsync(id, GetToken());
            return RedirectToAction(nameof(Index));
        }
    }
}
