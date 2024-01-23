using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeWorkController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
