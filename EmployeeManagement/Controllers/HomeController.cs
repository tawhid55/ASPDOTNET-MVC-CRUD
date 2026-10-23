using Microsoft.AspNetCore.Authorization;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmployeeDbContext demoDbContext;

        public HomeController(ILogger<HomeController> logger, EmployeeDbContext demoDbContext)
        {
            _logger = logger;
            this.demoDbContext = demoDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await demoDbContext.Employees.ToListAsync();

            ViewBag.EmployeeCount = employees.Count;
            ViewBag.DepartmentCount = employees
                .Select(employee => employee.Department)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            ViewBag.PayrollTotal = employees.Sum(employee => employee.Salary);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
