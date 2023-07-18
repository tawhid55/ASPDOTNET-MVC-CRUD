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

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var employe = await demoDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if(employe != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employe.Id,
                    Name = employe.Name,
                    Email = employe.Email,
                    Salary = employe.Salary,
                    Department = employe.Department,
                    DateOfBirth = employe.DateOfBirth
                };

                return await Task.Run(() => View("View", viewModel));
            }

            

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel updateEmployeeViewModel)
        {
            var employee = await demoDbContext.Employees.FindAsync(updateEmployeeViewModel.Id);
            
            if(employee != null)
            {
                employee.Name = updateEmployeeViewModel.Name;
                employee.Email = updateEmployeeViewModel.Email;
                employee.Salary = updateEmployeeViewModel.Salary;
                employee.DateOfBirth = updateEmployeeViewModel.DateOfBirth;
                employee.Department = updateEmployeeViewModel.Department;

                await demoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
                
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel updateEmployeeViewModel)
        {
            var employee = await demoDbContext.Employees.FindAsync(updateEmployeeViewModel.Id);

            if(employee != null)
            {
                demoDbContext.Employees.Remove(employee);
                await demoDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
