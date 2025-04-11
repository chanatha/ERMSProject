using Xunit;
using Moq;
using ERMS.API.Controllers;
using ERMS.API.Interfaces;
using ERMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class ProjectsControllerApiTests
{
    private readonly Mock<IProjectData> _mockRepo;
    private readonly ProjectsController _controller;

    public ProjectsControllerApiTests()
    {
        _mockRepo = new Mock<IProjectData>();
        _controller = new ProjectsController(_mockRepo.Object);
    }

    [Fact]
    public void GetAll_ReturnsProjects()
    {
        var projects = new List<Project> { new Project { Id = 1, Name = "Test Project" } };
        _mockRepo.Setup(r => r.GetAllProjects()).Returns(projects);

        var result = _controller.GetAll();

        var actionResult = Assert.IsType<ActionResult<List<Project>>>(result);
        Assert.Single(actionResult.Value);
    }

    [Fact]
    public void GetById_ReturnsProject()
    {
        var project = new Project { Id = 1, Name = "Project 1" };
        _mockRepo.Setup(r => r.GetProjectById(1)).Returns(project);

        var result = _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(project, okResult.Value);
    }

    [Fact]
    public void CreateProject_ReturnsProject()
    {
        var project = new Project { Name = "New Project" };
        var result = _controller.CreateProject(project);

        var created = Assert.IsType<Project>(result.Value);
        Assert.Equal("New Project", created.Name);
    }

    [Fact]
    public void EditProject_ReturnsUpdatedProject()
    {
        var project = new Project { Id = 1, Name = "Updated Project" };
        _mockRepo.Setup(r => r.UpdateProject(1, project)).Returns(project);

        var result = _controller.EditProject(1, project);

        var updated = Assert.IsType<Project>(result.Value);
        Assert.Equal("Updated Project", updated.Name);
    }

    [Fact]
    public void DeleteProject_ReturnsDeletedProject()
    {
        var project = new Project { Id = 1, Name = "To Delete" };
        _mockRepo.Setup(r => r.GetProjectById(1)).Returns(project);
        _mockRepo.Setup(r => r.DeleteProject(1)).Returns(true);

        var result = _controller.DeleteProject(1);

        var actionResult = Assert.IsType<ActionResult<Project>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedProject = Assert.IsType<Project>(okResult.Value);
        Assert.Equal(project.Id, returnedProject.Id);
    }

}
