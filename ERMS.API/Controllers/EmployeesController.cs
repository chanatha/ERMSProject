using ERMS.API.Interfaces;
using ERMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ERMS.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeData _data;

        public EmployeesController(IEmployeeData data)
        {
            _data = data;
        }

        [HttpGet("/api/Employees")]
        public ActionResult<List<Employee>> GetAll()
        {
            return _data.GetAllEmployees();
        }

        [HttpGet("/api/Employee/{id}", Name = "GetEmployee")]
        public ActionResult<Employee> GetById(int? id)
        {
            var item = _data.GetEmployeeById(id);
            if (item == null)
                return NotFound();

            return new ObjectResult(item);
        }

        [HttpPost("/api/createEmployee", Name = "CreateEmployee")]
        public ActionResult<Employee> CreateEmployee([FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest("Employee is null");

            _data.CreateEmployee(employee);
            return employee;
        }

        [HttpPut("/api/editEmployee/{id}", Name = "EditEmployee")]
        public ActionResult<Employee> EditEmployee(int? id, [FromBody] Employee employee)
        {
            Employee editedEmployee = _data.UpdateEmployee(id, employee);
            if (editedEmployee is null || id != employee.Id)
                return BadRequest("Employee does not exist or ID mismatch");

            return editedEmployee;
        }

        [HttpDelete("/api/deleteEmployee/{id}", Name = "DeleteEmployee")]
        public virtual ActionResult<Employee> DeleteById(int? id)
        {
            var item = _data.GetEmployeeById(id);
            if (item != null && _data.DeleteEmployee(id))
            {
                return new ObjectResult(item);
            }

            return NotFound();
        }
    }
}
