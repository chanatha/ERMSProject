// File: Interfaces/IWorkTaskData.cs
using ERMS.API.Models;
using System.Collections.Generic;

namespace ERMS.API.Interfaces
{
    public interface IWorkTaskData
    {
        List<WorkTask> GetAllTasks();
        WorkTask GetTaskById(int? id);
        void CreateTask(WorkTask task);
        WorkTask UpdateTask(int? id, WorkTask task);
        bool DeleteTask(int? id);
    }
}
