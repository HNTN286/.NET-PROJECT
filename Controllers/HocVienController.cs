using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrungTamDaoTao.Data;
using TrungTamDaoTao.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace TrungTamDaoTao.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class HocVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HocVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HocVien
        public async Task<IActionResult> Index()
        {
            return View(await _context.HocVien.ToListAsync());
        }

        // GET: HocVien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocVien
                .Include(h => h.DangKyKhoaHoc)
                .ThenInclude(d => d.KhoaHoc)
                .FirstOrDefaultAsync(m => m.MaHocVien == id);

            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // GET: HocVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HocVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHocVien,HoTen,NgaySinh,SoDienThoai,Email,TaiKhoan,PasswordHash")] HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem MaHocVien đã tồn tại chưa
                var existingMaHocVien = await _context.HocVien.FirstOrDefaultAsync(h => h.MaHocVien == hocVien.MaHocVien);
                if (existingMaHocVien != null)
                {
                    ModelState.AddModelError("MaHocVien", "Mã học viên đã tồn tại!");
                    return View(hocVien);
                }

                // Kiểm tra xem TaiKhoan đã tồn tại chưa
                var existingTaiKhoan = await _context.HocVien.FirstOrDefaultAsync(h => h.TaiKhoan == hocVien.TaiKhoan);
                if (existingTaiKhoan != null)
                {
                    ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại!");
                    return View(hocVien);
                }

                // Kiểm tra xem Email đã tồn tại chưa
                var existingEmail = await _context.HocVien.FirstOrDefaultAsync(h => h.Email == hocVien.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại!");
                    return View(hocVien);
                }

                // Mã hóa mật khẩu trước khi lưu
                hocVien.PasswordHash = HashPassword(hocVien.PasswordHash);
                hocVien.VaiTro = "HocVien";
                
                _context.Add(hocVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hocVien);
        }

        // GET: HocVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocVien.FindAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }
            
            // Không hiển thị mật khẩu thực sự
            ViewBag.PasswordPlaceholder = "••••••••";
            return View(hocVien);
        }

        // POST: HocVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHocVien,HoTen,NgaySinh,SoDienThoai,Email,TaiKhoan,PasswordHash,VaiTro")] HocVien hocVien, string currentPassword)
        {
            if (id != hocVien.MaHocVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin học viên hiện tại
                    var currentHocVien = await _context.HocVien.FindAsync(id);
                    if (currentHocVien == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra xem tài khoản hoặc email đã tồn tại chưa (nếu thay đổi)
                    if (currentHocVien.TaiKhoan != hocVien.TaiKhoan)
                    {
                        var existingTaiKhoan = await _context.HocVien.FirstOrDefaultAsync(h => h.TaiKhoan == hocVien.TaiKhoan);
                        if (existingTaiKhoan != null)
                        {
                            ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại!");
                            return View(hocVien);
                        }
                    }

                    if (currentHocVien.Email != hocVien.Email)
                    {
                        var existingEmail = await _context.HocVien.FirstOrDefaultAsync(h => h.Email == hocVien.Email);
                        if (existingEmail != null)
                        {
                            ModelState.AddModelError("Email", "Email đã tồn tại!");
                            return View(hocVien);
                        }
                    }

                    // Cập nhật thông tin
                    currentHocVien.HoTen = hocVien.HoTen;
                    currentHocVien.NgaySinh = hocVien.NgaySinh;
                    currentHocVien.SoDienThoai = hocVien.SoDienThoai;
                    currentHocVien.Email = hocVien.Email;
                    currentHocVien.TaiKhoan = hocVien.TaiKhoan;
                    currentHocVien.VaiTro = hocVien.VaiTro;

                    // Chỉ cập nhật mật khẩu nếu có nhập mật khẩu mới
                    if (!string.IsNullOrEmpty(hocVien.PasswordHash) && hocVien.PasswordHash != "••••••••")
                    {
                        currentHocVien.PasswordHash = HashPassword(hocVien.PasswordHash);
                    }

                    _context.Update(currentHocVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocVienExists(hocVien.MaHocVien))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hocVien);
        }

        // GET: HocVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocVien
                .FirstOrDefaultAsync(m => m.MaHocVien == id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // POST: HocVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hocVien = await _context.HocVien.FindAsync(id);
            
            // Kiểm tra xem học viên có đăng ký khóa học nào không
            var dangKyKhoaHoc = await _context.DangKyKhoaHoc.AnyAsync(d => d.MaHocVien == id);
            if (dangKyKhoaHoc)
            {
                TempData["ErrorMessage"] = "Không thể xóa học viên đã đăng ký khóa học!";
                return RedirectToAction(nameof(Index));
            }
            
            if (hocVien != null)
            {
                _context.HocVien.Remove(hocVien);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool HocVienExists(string id)
        {
            return _context.HocVien.Any(e => e.MaHocVien == id);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}