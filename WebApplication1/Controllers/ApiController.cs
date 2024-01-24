using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using WebApplication1.Models;
using WebApplication1.Models.Dto;

namespace WebApplication1.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _dBContext;
        private readonly IWebHostEnvironment _host;
        public ApiController(MyDBContext dBContext, IWebHostEnvironment host)
        {
            _dBContext = dBContext;
            _host = host;
        }

        public IActionResult Index()
        {
            System.Threading.Thread.Sleep(5000);// 延遲5秒

            #region 連線至API格式
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
            #endregion  
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

        public IActionResult HomeWork2(string name)
        {
            // todo 判斷資料庫使否有使用者
            Member? member = _dBContext.Members.Where(p => p.Name == name).FirstOrDefault();
            if (member != null)
            {
                return Content($"{name}已註冊");
            }
            return Content($"{name}可以註冊");
        }

        //public IActionResult Register(string name,int age = 20)
        //public IActionResult Register(MemberDto member) //實作1 2
        public IActionResult Register(Member member, IFormFile Avatar) //實作3
        {
            //未取得使用者判斷
            if (string.IsNullOrEmpty(member.Name))
            {
                member.Name = "Guest";
            }

            #region todo
            ////todo 檔案存在的處理
            //string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            //if (!Directory.Exists(uploadPath))
            //{
            //    Directory.CreateDirectory(uploadPath);
            //}
            ////todo 限制檔案上傳類型
            //var fileExtension = Path.GetExtension(member.Avatar.FileName);
            //if (fileExtension != ".jpg")
            //{
            //    return Content("檔案格式錯誤");
            //}
            ////todo 限制檔案大小
            //if (member.Avatar.Length > 1024 * 1024)
            //{
            //    return Content("檔案大小超過限制");
            //}
            //儲存上傳檔案
            #endregion

            #region 實作1 指定路徑的上傳方式
            //指定路徑
            //string uploadPath = @"C:\Users\ispan\source\repos\projects\Ajax\20240122\20240122.solution\WebApplication1\wwwroot\uploads\fileName.jpg";
            //using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            //{
            //    //拷貝檔案
            //    member.Avatar?.CopyTo(fileStream);
            //}
            #endregion

            #region 實作2 取得Server端檔案路徑方式
            //指定檔案名稱
            //string fileName = "empty.jpg";
            //if (member.Avatar != null)
            //{
            //    fileName = member.Avatar.FileName;
            //}
            ////取得檔案上傳實際路徑， Path.Combine 合併路徑
            //string uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);
            ////上傳檔案
            //using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            //{
            //    member.Avatar?.CopyTo(fileStream);
            //}
            #endregion

            #region 實作3 上傳檔案轉換成2進位碼方式
            string fileName = "empty.jpg";
            if (Avatar != null)
            {
                fileName = Avatar.FileName;
            }
            string uploadPath = Path.Combine(_host.WebRootPath, "uploads", fileName);
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                Avatar?.CopyTo(fileStream);
            }
            //test

            //轉成二進位
            byte[]? imgByte = null;
            using (var memoryStream = new MemoryStream())
            {
                Avatar?.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }
            member.FileName = fileName;
            member.FileData = imgByte;
            #endregion

            #region return
            //回傳填入input的值
            //return Content($"Hello {member.Name}，{member.Age} 歲了，電子郵件是{member.Email}");

            //回傳上傳檔案資訊
            //return Content($"{member.Avatar?.FileName}-{member.Avatar?.Length}-{member.Avatar?.ContentType}");

            //實作1 回傳檔案上傳路徑
            //return Content(Path.Combine(_host.WebRootPath, "uploads", fileName));

            //實作3 回傳檔案二進位
            return Content("上傳成功");
            #endregion
        }

        #region 排序景點與搜尋
        // Server 接收資料
        [HttpPost]
        public IActionResult Spots([FromBody] SearchDto _search)
        {
            // 根據分類編號讀取景點資料
            var spots = _search.categoryId == 0
                ? _dBContext.SpotImagesSpots
                : _dBContext.SpotImagesSpots.Where(p => p.CategoryId == _search.categoryId);

            //根據關鍵字搜尋
            if (!string.IsNullOrEmpty(_search.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.keyword)||s.SpotDescription.Contains(_search.keyword));
            }

            //排序
            switch (_search.sortBy)
            {
                case "spotTitle":
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //分頁
            int totalCount = spots.Count(); // 搜尋出來的資料共有幾筆
            int pageSize = _search.pageSize ?? 9; //每頁多少筆資料 預設9筆
            int totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize); //共有幾頁
            int page = _search.page ?? 1; //現在在第幾頁

            //取出分頁資料
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);

            //設計要回傳的資料格式
            SpotsPagingDto spotsPaging = new SpotsPagingDto();
            spotsPaging.TotalPages = totalPage;
            spotsPaging.SpotsResult = spots.ToList();

            return Json(spotsPaging);
            //return Json(spots);//檢查 Json 讀取頁面
            //return Json(_search);
            //return Content("spots");
        }
        #endregion
    }
}
