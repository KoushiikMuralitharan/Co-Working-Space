using Microsoft.AspNetCore.Mvc;

namespace CoWorkingSpace.Api.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
