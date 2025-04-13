using ERMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<WorkTask> Tasks { get; set; }
        public DbSet<Test> Tests { get; set; }

        // 🔽 WorkTask methods
        public async Task<List<WorkTask>> GetAllWorkTasksAsync()
            => await Tasks.FromSqlRaw("EXEC GetAllWorkTasks").ToListAsync();

        public async Task<WorkTask> GetWorkTaskByIdAsync(int id)
            => await Tasks.FromSqlRaw("EXEC GetWorkTaskById @Id = {0}", id).FirstOrDefaultAsync();

        public async Task<int> InsertWorkTaskAsync(WorkTask task)
            => await Database.ExecuteSqlRawAsync("EXEC InsertWorkTask @Title = {0}, @Description = {1}, @Status = {2}, @Priority = {3}, @DueDate = {4}, @ProjectId = {5}, @EmployeeId = {6}",
                task.Title, task.Description, task.Status, task.Priority, task.DueDate, task.ProjectId, task.EmployeeId);

        public async Task<int> UpdateWorkTaskAsync(WorkTask task)
            => await Database.ExecuteSqlRawAsync("EXEC UpdateWorkTask @Id = {0}, @Title = {1}, @Description = {2}, @Status = {3}, @Priority = {4}, @DueDate = {5}, @ProjectId = {6}, @EmployeeId = {7}",
                task.Id, task.Title, task.Description, task.Status, task.Priority, task.DueDate, task.ProjectId, task.EmployeeId);

        public async Task<int> DeleteWorkTaskAsync(int id)
            => await Database.ExecuteSqlRawAsync("EXEC DeleteWorkTask @Id = {0}", id);

        // 🔽 Employee methods
        public async Task<List<Employee>> GetAllEmployeesAsync()
            => await Employees.FromSqlRaw("EXEC GetAllEmployees").ToListAsync();

        public async Task<Employee> GetEmployeeByIdAsync(int id)
            => await Employees.FromSqlRaw("EXEC GetEmployeeById @Id = {0}", id).FirstOrDefaultAsync();

        public async Task<int> InsertEmployeeAsync(Employee emp)
            => await Database.ExecuteSqlRawAsync("EXEC InsertEmployee @FirstName = {0}, @LastName = {1}, @Email = {2}, @Department = {3}, @Position = {4}, @HireDate = {5}",
                emp.FirstName, emp.LastName, emp.Email, emp.Department, emp.Position, emp.HireDate);

        public async Task<int> UpdateEmployeeAsync(Employee emp)
            => await Database.ExecuteSqlRawAsync("EXEC UpdateEmployee @Id = {0}, @FirstName = {1}, @LastName = {2}, @Email = {3}, @Department = {4}, @Position = {5}, @HireDate = {6}",
                emp.Id, emp.FirstName, emp.LastName, emp.Email, emp.Department, emp.Position, emp.HireDate);

        public async Task<int> DeleteEmployeeAsync(int id)
            => await Database.ExecuteSqlRawAsync("EXEC DeleteEmployee @Id = {0}", id);

        // 🔽 Project methods
        public async Task<List<Project>> GetAllProjectsAsync()
            => await Projects.FromSqlRaw("EXEC GetAllProjects").ToListAsync();

        public async Task<Project> GetProjectByIdAsync(int id)
            => await Projects.FromSqlRaw("EXEC GetProjectById @Id = {0}", id).FirstOrDefaultAsync();

        public async Task<int> InsertProjectAsync(Project project)
            => await Database.ExecuteSqlRawAsync("EXEC InsertProject @Name = {0}, @Description = {1}, @StartDate = {2}, @EndDate = {3}",
                project.Name, project.Description, project.StartDate, project.EndDate);

        public async Task<int> UpdateProjectAsync(Project project)
            => await Database.ExecuteSqlRawAsync("EXEC UpdateProject @Id = {0}, @Name = {1}, @Description = {2}, @StartDate = {3}, @EndDate = {4}",
                project.Id, project.Name, project.Description, project.StartDate, project.EndDate);

        public async Task<int> DeleteProjectAsync(int id)
            => await Database.ExecuteSqlRawAsync("EXEC DeleteProject @Id = {0}", id);
    }

}