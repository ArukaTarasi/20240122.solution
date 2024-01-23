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
            // 延遲5秒
            System.Threading.Thread.Sleep(5000);

            //return View();
            //return Json("Hello Content"); // 回傳純文字

            // 連線至API /api/index
            // 傳入文字的第二個參數是用什麼格式的資料
            // 傳入文字的第三個參數是用什麼方式做編碼
            // return Content("1","2",3);
            // return Content("<h2>Hello Content</h2>"); // 還傳HTML標籤，沒有意義 會還傳整段文字
            // return Content("<h2>Hello Content 你好</h2>", "text/plain"); // plain 純文字格式，插入中文會變成亂碼
            return Content("Content 你好", "text/plain", System.Text.Encoding.UTF8); // 加上編碼方式
            // return Content("<h2>Hello Content</h2>", "text/html"); // 給第二個參數 "text/html" 指定格式就能使用HTML

            // return Content(new { name = "Hello Content" }); // 回傳物件
        }

        //[HttpPost]// HttpPost 就無法使用API測試
        public IActionResult Cities()
        {

            var products = _dBContext.Addresses.Select(p => p.City).Distinct();
            return Json(products);
            //return Json(products);
        }

        public IActionResult Districts(string city)
        {
            //根據City，取得不重複的 SiteId 放入 districts
            var districts = _dBContext.Addresses.Where(p => p.City == city).Select(p => p.SiteId).Distinct();
            return Json(districts);
        }

        public IActionResult Avatar(int id = 1)
        {
            Member? member = _dBContext.Members.Find(id);

            if (member != null)
            {
                byte[]? img = member.FileData;
                return File(img, "image/jpeg");
            }
            return NotFound();
        }

        public IActionResult Register(string name,int age = 20)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "Guest";
            }
            return Content($"Hello {name} m You are {age} years old");
        }
    }
}
