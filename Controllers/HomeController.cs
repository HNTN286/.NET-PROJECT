using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamDaoTao.Data;
using TrungTamDaoTao.Models;

namespace TrungTamDaoTao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminIndex()
        {
            // Nếu người dùng là QuanTriVien, chuyển hướng họ đến trang Dashboard
            if (User.IsInRole("QuanTriVien"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            
            // Người dùng khác vẫn xem được trang Home
            return View();
        }

        public async Task<IActionResult> Index()
{
    // Kiểm tra vai trò và chuyển hướng nếu là admin
    if (User.IsInRole("QuanTriVien"))
    {
        return RedirectToAction("Dashboard", "Admin");
    }
    
    // Tiếp tục với logic thông thường cho người dùng không phải admin
    var khoaHocSapKhaiGiang = await _context.KhoaHoc
        .Where(k => k.ThoiGianKhaiGiang >= DateTime.Now)
        .OrderBy(k => k.ThoiGianKhaiGiang)
        .Take(5)
        .ToListAsync();
    
    return View(khoaHocSapKhaiGiang);
}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}