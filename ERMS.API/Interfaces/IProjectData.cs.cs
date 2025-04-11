// File: Interfaces/IProjectData.cs
using ERMS.API.Models;
using System.Collections.Generic;

namespace ERMS.API.Interfaces
{
    public interface IProjectData
    {
        List<Project> GetAllProjects();
        Project GetProjectById(int? id);
        void CreateProject(Project project);
        Project UpdateProject(int? id, Project project);
        bool DeleteProject(int? id);
    }
}
