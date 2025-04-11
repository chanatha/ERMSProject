using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ERMS.Controllers;
using ERMS.Models;
using ERMS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERMS.Tests
{
    public class ProjectsControllerTests
    {
        private readonly Mock<ProjectApiService> _mockService;
        private readonly ProjectsController _controller;

        public ProjectsControllerTests()
        {
            _mockService = new Mock<ProjectApiService>(MockBehavior.Strict, null!);
            _controller = new ProjectsController(_mockService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithProjects()
        {
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Project A" },
                new Project { Id = 2, Name = "Project B" }
            };

            _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>()))
                        .ReturnsAsync(projects);

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Project>>(view.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_ReturnsProjectView()
        {
            var project = new Project { Id = 1, Name = "Test Project" };

            _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<string>()))
                        .ReturnsAsync(project);

            var result = await _controller.Details(1);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(project, view.Model);
        }

        [Fact]
        public async Task Create_ValidProject_RedirectsToIndex()
        {
            var project = new Project
            {
                Id = 1,
                Name = "New Project",
                Description = "Some details",
                StartDate = System.DateTime.Today
            };

            _mockService.Setup(s => s.CreateAsync(project, It.IsAny<string>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.Create(project);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("Title", "Required");

            var project = new Project();

            var result = await _controller.Create(project);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(project, view.Model);
        }
    }
}
