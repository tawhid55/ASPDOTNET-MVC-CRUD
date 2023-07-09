using ASPDOTNET_MVC_CRUD.Data;
using ASPDOTNET_MVC_CRUD.Models;
using ASPDOTNET_MVC_CRUD.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPDOTNET_MVC_CRUD.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly DemoDbContext demoDbContext;

        public EmployeesController(DemoDbContext demoDbContext)
        {
            this.demoDbContext = demoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await demoDbContext.Employees.ToListAsync();
            return View(employees);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)
        {
            var employe = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                Department = addEmployeeRequest.Department,
                DateOfBirth = addEmployeeRequest.DateOfBirth
            };

            await demoDbContext.Employees.AddAsync(employe);
            await demoDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}
