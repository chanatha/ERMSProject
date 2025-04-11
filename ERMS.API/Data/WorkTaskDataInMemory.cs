// File: Data/WorkTaskDataInMemory.cs
using ERMS.API.Interfaces;
using ERMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERMS.API.Data
{
    public class WorkTaskDataInMemory : IWorkTaskData
    {
        private static List<WorkTask> _tasks = new List<WorkTask>
        {
            new WorkTask
            {
                Id = 1,
                Title = "Set up project structure",
                Description = "Initialize folders and solution",
                Status = "To Do",
                Priority = "High",
                DueDate = DateTime.Today.AddDays(5),
                ProjectId = 1,
                EmployeeId = 1
            }
        };

        private static int _nextId = _tasks.Max(t => t.Id) + 1;

        public List<WorkTask> GetAllTasks() => _tasks;

        public WorkTask GetTaskById(int? id) => _tasks.FirstOrDefault(t => t.Id == id);

        public void CreateTask(WorkTask task)
        {
            task.Id = _nextId++;

            if (task.Employee != null)
                task.EmployeeId = task.Employee.Id;

            if (task.Project != null)
                task.ProjectId = task.Project.Id;

            _tasks.Add(task);
        }

        public WorkTask UpdateTask(int? id, WorkTask task)
        {
            var existing = GetTaskById(id);
            if (existing == null) return null;

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Status = task.Status;
            existing.Priority = task.Priority;
            existing.DueDate = task.DueDate;

            if (task.Project != null)
            {
                existing.ProjectId = task.Project.Id;
                existing.Project = task.Project;
            }

            if (task.Employee != null)
            {
                existing.EmployeeId = task.Employee.Id;
                existing.Employee = task.Employee;
            }

            return existing;
        }


        public bool DeleteTask(int? id)
        {
            var existing = GetTaskById(id);
            if (existing != null)
            {
                _tasks.Remove(existing);
                return true;
            }
            return false;
        }
    }
}
