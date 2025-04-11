using Xunit;
using Moq;
using ERMS.Controllers;
using ERMS.Models;
using ERMS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERMS.Tests
{
    public class EmployeesControllerTests
    {
        private readonly Mock<EmployeeApiService> _mockService;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockService = new Mock<EmployeeApiService>(MockBehavior.Strict, null!);
            _controller = new EmployeesController(_mockService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithEmployees()
        {
            // Arrange
            var employees = new List<Employee> {
                new Employee { Id = 1, FirstName = "John", LastName = "Doe" }
            };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<string>()))
                        .ReturnsAsync(employees);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Employee>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_IdIsNull_ReturnsNotFound()
        {
            var result = await _controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsView()
        {
            var emp = new Employee { Id = 1, FirstName = "Jane", LastName = "Doe" };
            _mockService.Setup(s => s.GetByIdAsync(1, It.IsAny<string>()))
                        .ReturnsAsync(emp);

            var result = await _controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(emp, viewResult.Model);
        }

        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            var emp = new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john@example.com",
                Department = "HR",
                Position = "Manager",
                HireDate = System.DateTime.Today
            };

            _mockService.Setup(s => s.CreateAsync(emp, It.IsAny<string>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.Create(emp);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_IdMismatch_ReturnsNotFound()
        {
            var emp = new Employee { Id = 2 };
            var result = await _controller.Edit(1, emp);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesEmployeeAndRedirects()
        {
            _mockService.Setup(s => s.DeleteAsync(1, It.IsAny<string>()))
                        .Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}
