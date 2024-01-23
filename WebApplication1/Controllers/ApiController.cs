using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _dBContext;
        public ApiController(MyDBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Cities()
        {

            var products = _dBContext.Addresses.Select(p => p.City).Distinct();

            return Json(products);
        }
    }
}
