using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ERMS.Models;
using ERMS.Services;
using ERMS.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ERMS.Tests
{
    public class WorkTasksControllerTests
    {
        private readonly Mock<WorkTaskApiService> _mockTaskService;
        private readonly Mock<ProjectApiService> _mockProjectService;
        private readonly Mock<EmployeeApiService> _mockEmployeeService;
        private readonly WorkTasksController _controller;

        public WorkTasksControllerTests()
        {
            _mockTaskService = new Mock<WorkTaskApiService>(MockBehavior.Strict, null!);
            _mockProjectService = new Mock<ProjectApiService>(MockBehavior.Strict, null!);
            _mockEmployeeService = new Mock<EmployeeApiService>(MockBehavior.Strict, null!);

            _controller = new WorkTasksController(
                _mockTaskService.Object,
                _mockProjectService.Object,
                _mockEmployeeService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithTasks()
        {
            var tasks = new List<WorkTask>
            {
                new WorkTask { Id = 1, Title = "Task 1", Status = "To Do", Priority = "High", ProjectId = 1 },
                new WorkTask { Id = 2, Title = "Task 2", Status = "In Progress", Priority = "Medium", ProjectId = 2 }
            };

            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project A" },
                new Project { Id = 2, Name = "Project B" }
            };

            var employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "Jane", LastName = "Doe" }
            };

            _mockTaskService.Setup(s => s.GetAllAsync(It.IsAny<string>())).ReturnsAsync(tasks);
            _mockProjectService.Setup(p => p.GetAllAsync(It.IsAny<string>())).ReturnsAsync(projects);
            _mockEmployeeService.Setup(e => e.GetAllAsync(It.IsAny<string>())).ReturnsAsync(employees);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<WorkTask>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_ReturnsCorrectTask()
        {
            var task = new WorkTask { Id = 1, Title = "Test Task", ProjectId = 1, EmployeeId = 1 };
            var project = new Project { Id = 1, Name = "Test Project" };
            var employee = new Employee { Id = 1, FirstName = "Jane", LastName = "Doe" };

            _mockTaskService.Setup(t => t.GetByIdAsync(1, It.IsAny<string>())).ReturnsAsync(task);
            _mockProjectService.Setup(p => p.GetByIdAsync(1, It.IsAny<string>())).ReturnsAsync(project);
            _mockEmployeeService.Setup(e => e.GetByIdAsync(1, It.IsAny<string>())).ReturnsAsync(employee);

            var result = await _controller.Details(1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<WorkTask>(view.Model);
            Assert.Equal("Test Task", model.Title);
        }

        [Fact]
        public async Task Create_ValidTask_RedirectsToIndex()
        {
            var task = new WorkTask
            {
                Title = "New Task",
                Description = "Details",
                Status = "To Do",
                Priority = "High",
                DueDate = DateTime.Today.AddDays(3),
                ProjectId = 1
            };

            _mockTaskService.Setup(s => s.CreateAsync(task, It.IsAny<string>()))
                            .Returns(Task.CompletedTask);

            // Also mock PopulateDropdowns (for fallback when model is invalid)
            _mockProjectService.Setup(p => p.GetAllAsync(It.IsAny<string>())).ReturnsAsync(new List<Project>());
            _mockEmployeeService.Setup(e => e.GetAllAsync(It.IsAny<string>())).ReturnsAsync(new List<Employee>());

            var result = await _controller.Create(task);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsView()
        {
            var task = new WorkTask(); // Invalid

            _controller.ModelState.AddModelError("Title", "Required");

            _mockProjectService.Setup(p => p.GetAllAsync(It.IsAny<string>())).ReturnsAsync(new List<Project>());
            _mockEmployeeService.Setup(e => e.GetAllAsync(It.IsAny<string>())).ReturnsAsync(new List<Employee>());

            var result = await _controller.Create(task);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(task, view.Model);
        }
    }
}
