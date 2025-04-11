using ERMS.API.Interfaces;
using ERMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERMS.API.Data
{
    public class EmployeeDataInMemory : IEmployeeData
    {
        private static List<Employee> _employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com",
                Department = "HR",
                Position = "Recruiter",
                HireDate = new DateTime(2021, 5, 10)
            },
            new Employee
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bob.smith@example.com",
                Department = "IT",
                Position = "Developer",
                HireDate = new DateTime(2020, 3, 22)
            },
            new Employee
            {
                Id = 3,
                FirstName = "Carol",
                LastName = "Williams",
                Email = "carol.williams@example.com",
                Department = "Finance",
                Position = "Accountant",
                HireDate = new DateTime(2019, 8, 15)
            }
        };

        private static int _nextId = _employees.Max(e => e.Id) + 1;

        public List<Employee> GetAllEmployees() => _employees;

        public Employee GetEmployeeById(int? id) => _employees.FirstOrDefault(e => e.Id == id);

        public void CreateEmployee(Employee employee)
        {
            employee.Id = _nextId++;
            _employees.Add(employee);
        }

        public Employee UpdateEmployee(int? id, Employee employee)
        {
            var existing = GetEmployeeById(id);
            if (existing == null) return null;

            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Email = employee.Email;
            existing.Department = employee.Department;
            existing.Position = employee.Position;
            existing.HireDate = employee.HireDate;

            return existing;
        }

        public bool DeleteEmployee(int? id)
        {
            var existing = GetEmployeeById(id);
            if (existing != null)
            {
                _employees.Remove(existing);
                return true;
            }
            return false;
        }
    }
}
