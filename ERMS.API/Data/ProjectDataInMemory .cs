// File: Data/ProjectDataInMemory.cs
using ERMS.API.Interfaces;
using ERMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERMS.API.Data
{
    public class ProjectDataInMemory : IProjectData
    {
        private static List<Project> _projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                Name = "ERP Implementation",
                Description = "Deploying ERP system company-wide.",
                StartDate = new DateTime(2024, 1, 15),
                EndDate = new DateTime(2024, 6, 15)
            }
        };

        private static int _nextId = _projects.Max(p => p.Id) + 1;

        public List<Project> GetAllProjects() => _projects;

        public Project GetProjectById(int? id) => _projects.FirstOrDefault(p => p.Id == id);

        public void CreateProject(Project project)
        {
            project.Id = _nextId++;
            _projects.Add(project);
        }

        public Project UpdateProject(int? id, Project project)
        {
            var existing = GetProjectById(id);
            if (existing == null) return null;

            existing.Name = project.Name;
            existing.Description = project.Description;
            existing.StartDate = project.StartDate;
            existing.EndDate = project.EndDate;

            return existing;
        }

        public bool DeleteProject(int? id)
        {
            var existing = GetProjectById(id);
            if (existing != null)
            {
                _projects.Remove(existing);
                return true;
            }
            return false;
        }
    }
}
