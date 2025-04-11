using Xunit;
using Moq;
using ERMS.API.Controllers;
using ERMS.API.Interfaces;
using ERMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class WorkTasksControllerApiTests
{
    private readonly Mock<IWorkTaskData> _mockRepo;
    private readonly WorkTasksController _controller;

    public WorkTasksControllerApiTests()
    {
        _mockRepo = new Mock<IWorkTaskData>();
        _controller = new WorkTasksController(_mockRepo.Object);
    }

    [Fact]
    public void GetAll_ReturnsTasks()
    {
        var tasks = new List<WorkTask> { new WorkTask { Id = 1, Title = "Task A" } };
        _mockRepo.Setup(r => r.GetAllTasks()).Returns(tasks);

        var result = _controller.GetAll();

        var actionResult = Assert.IsType<ActionResult<List<WorkTask>>>(result);
        Assert.Single(actionResult.Value);
    }

    [Fact]
    public void GetById_ReturnsTask()
    {
        var task = new WorkTask { Id = 1, Title = "My Task" };
        _mockRepo.Setup(r => r.GetTaskById(1)).Returns(task);

        var result = _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(task, okResult.Value);
    }

    [Fact]
    public void Create_ReturnsTask()
    {
        var task = new WorkTask { Title = "New Task" };
        var result = _controller.Create(task);

        var created = Assert.IsType<WorkTask>(result.Value);
        Assert.Equal("New Task", created.Title);
    }

    [Fact]
    public void Edit_ReturnsUpdatedTask()
    {
        var task = new WorkTask { Id = 1, Title = "Updated Task" };
        _mockRepo.Setup(r => r.UpdateTask(1, task)).Returns(task);

        var result = _controller.Edit(1, task);

        var updated = Assert.IsType<WorkTask>(result.Value);
        Assert.Equal("Updated Task", updated.Title);
    }

    [Fact]
    public void Delete_ReturnsDeletedTask()
    {
        var task = new WorkTask { Id = 1, Title = "Delete Me" };
        _mockRepo.Setup(r => r.GetTaskById(1)).Returns(task);
        _mockRepo.Setup(r => r.DeleteTask(1)).Returns(true);

        var result = _controller.Delete(1);

        var actionResult = Assert.IsType<ActionResult<WorkTask>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedTask = Assert.IsType<WorkTask>(okResult.Value);
        Assert.Equal(task.Id, returnedTask.Id);
    }

}
