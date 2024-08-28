using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications.EmployeeSpecs;
using Talabat.Core.Specifications.Products_Specs;

namespace Talabat.APIs.Controllers
{
    public class EmployeesController : BaseApiController
    {
        private readonly IGenaricRepository<Employee> _employeeRepo;

        public EmployeesController(IGenaricRepository<Employee> employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        [HttpGet]  // GET  : /api/Employees
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var spec = new EmployeeWithDepartmentSpecifications();
            var employees = await _employeeRepo.GetAllWithSpecAsync(spec);

            return Ok(employees);
        }

        [ProducesResponseType(typeof(Employee),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {

            var spec = new EmployeeWithDepartmentSpecifications(id);

            var employee = await _employeeRepo.GetWithSpecAsync(spec);


            if (employee is null)
                return NotFound(new ApiResponse(404));

            return Ok(employee);

        }
    }
}
