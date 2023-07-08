using Microsoft.AspNetCore.Mvc;

namespace ASPDOTNET_MVC_CRUD.Controllers
{
    public class EmployeesController : Controller
    {
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


    }
}
