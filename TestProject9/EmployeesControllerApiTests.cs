using Xunit;
using Moq;
using ERMS.API.Controllers;
using ERMS.API.Interfaces;
using ERMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class EmployeesControllerApiTests
{
    private readonly Mock<IEmployeeData> _mockRepo;
    private readonly EmployeesController _controller;

    public EmployeesControllerApiTests()
    {
        _mockRepo = new Mock<IEmployeeData>();
        _controller = new EmployeesController(_mockRepo.Object);
    }

    [Fact]
    public void GetAll_ReturnsEmployees()
    {
        var employees = new List<Employee> { new Employee { Id = 1, FirstName = "Test", LastName = "User" } };
        _mockRepo.Setup(r => r.GetAllEmployees()).Returns(employees);

        var result = _controller.GetAll();

        var actionResult = Assert.IsType<ActionResult<List<Employee>>>(result);
        Assert.Single(actionResult.Value);
    }

    [Fact]
    public void GetById_ReturnsEmployee()
    {
        var employee = new Employee { Id = 1, FirstName = "Test", LastName = "User" };
        _mockRepo.Setup(r => r.GetEmployeeById(1)).Returns(employee);

        var result = _controller.GetById(1);

        var okResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(employee, okResult.Value);
    }

    [Fact]
    public void CreateEmployee_ReturnsCreatedEmployee()
    {
        var employee = new Employee { FirstName = "New", LastName = "User" };
        var result = _controller.CreateEmployee(employee);

        var created = Assert.IsType<Employee>(result.Value);
        Assert.Equal("New", created.FirstName);
    }

    [Fact]
    public void EditEmployee_ReturnsUpdatedEmployee()
    {
        var updated = new Employee { Id = 1, FirstName = "Updated", LastName = "User" };
        _mockRepo.Setup(r => r.UpdateEmployee(1, updated)).Returns(updated);

        var result = _controller.EditEmployee(1, updated);

        var returned = Assert.IsType<Employee>(result.Value);
        Assert.Equal("Updated", returned.FirstName);
    }

    [Fact]
   
    public void DeleteEmployee_ReturnsDeletedEmployee()
    {
        var employee = new Employee { Id = 1, FirstName = "Delete", LastName = "Me" };
        _mockRepo.Setup(r => r.GetEmployeeById(1)).Returns(employee);
        _mockRepo.Setup(r => r.DeleteEmployee(1)).Returns(true);

        var result = _controller.DeleteById(1);

        var actionResult = Assert.IsType<ActionResult<Employee>>(result);
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        var returnedEmployee = Assert.IsType<Employee>(objectResult.Value);
        Assert.Equal(employee.Id, returnedEmployee.Id);
    }

}
