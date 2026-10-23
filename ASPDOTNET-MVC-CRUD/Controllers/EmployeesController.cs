using Microsoft.AspNetCore.Authorization;
using ASPDOTNET_MVC_CRUD.Data;
using ASPDOTNET_MVC_CRUD.Models;
using ASPDOTNET_MVC_CRUD.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPDOTNET_MVC_CRUD.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly DemoDbContext demoDbContext;

        public EmployeesController(DemoDbContext demoDbContext)
        {
            this.demoDbContext = demoDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var employee = await demoDbContext.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return employee == null ? NotFound() : View(employee);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await demoDbContext.Employees.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
            return View(employees);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(addEmployeeRequest);
            }

            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                Department = addEmployeeRequest.Department,
                DateOfBirth = addEmployeeRequest.DateOfBirth!.Value
            };

            await demoDbContext.Employees.AddAsync(employee);
            await demoDbContext.SaveChangesAsync();
            TempData["Success"] = "Employee record saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var employee = await demoDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = new UpdateEmployeeViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Salary = employee.Salary,
                Department = employee.Department,
                DateOfBirth = employee.DateOfBirth
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UpdateEmployeeViewModel updateEmployeeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(updateEmployeeViewModel);
            }

            var employee = await demoDbContext.Employees.FindAsync(updateEmployeeViewModel.Id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = updateEmployeeViewModel.Name;
            employee.Email = updateEmployeeViewModel.Email;
            employee.Salary = updateEmployeeViewModel.Salary;
            employee.DateOfBirth = updateEmployeeViewModel.DateOfBirth!.Value;
            employee.Department = updateEmployeeViewModel.Department;

            await demoDbContext.SaveChangesAsync();
            TempData["Success"] = "Employee record saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var employee = await demoDbContext.Employees.FindAsync(id);

            if (employee == null) return NotFound();

            demoDbContext.Employees.Remove(employee);
            await demoDbContext.SaveChangesAsync();

            TempData["Success"] = "Employee deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
