using ERMS.API.Models;
using System.Collections.Generic;

namespace ERMS.API.Interfaces
{
    public interface IEmployeeData
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployeeById(int? id);
        void CreateEmployee(Employee employee);
        Employee UpdateEmployee(int? id, Employee employee);
        bool DeleteEmployee(int? id);
    }
}
