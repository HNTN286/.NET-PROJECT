using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrungTamDaoTao.Data;
using TrungTamDaoTao.Models;

namespace TrungTamDaoTao.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoaHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: KhoaHoc - Hiển thị danh sách cho tất cả người dùng
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhoaHoc.ToListAsync());
        }

        // GET: KhoaHoc/Details/5 - Chi tiết khóa học, ai cũng xem được
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHoc
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            // Đếm số lượng học viên đã đăng ký
            ViewBag.SoLuongDangKy = await _context.DangKyKhoaHoc
                .Where(dk => dk.MaKhoaHoc == id && !dk.DaHuy)
                .CountAsync();

            return View(khoaHoc);
        }

        // GET: KhoaHoc/Create - Chỉ có người dùng đã đăng nhập mới được tạo
        [Authorize(Roles = "QuanTriVien")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhoaHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Create([Bind("MaKhoaHoc,TenKhoaHoc,GiangVien,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã khóa học đã tồn tại chưa
                var existingCourse = await _context.KhoaHoc.FirstOrDefaultAsync(k => k.MaKhoaHoc == khoaHoc.MaKhoaHoc);
                if (existingCourse != null)
                {
                    ModelState.AddModelError("MaKhoaHoc", "Mã khóa học đã tồn tại.");
                    return View(khoaHoc);
                }

                _context.Add(khoaHoc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm khóa học thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(khoaHoc);
        }

        // GET: KhoaHoc/Edit/5
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHoc.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            return View(khoaHoc);
        }

        // POST: KhoaHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Edit(string id, [Bind("MaKhoaHoc,TenKhoaHoc,GiangVien,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khoaHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật khóa học thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaHocExists(khoaHoc.MaKhoaHoc))
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
            return View(khoaHoc);
        }

        // GET: KhoaHoc/Delete/5
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHoc
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            // Kiểm tra xem khóa học đã có học viên đăng ký chưa
            var dangKy = await _context.DangKyKhoaHoc
                .AnyAsync(dk => dk.MaKhoaHoc == id && !dk.DaHuy);
            
            if (dangKy)
            {
                TempData["ErrorMessage"] = "Không thể xóa khóa học đã có học viên đăng ký!";
                return RedirectToAction(nameof(Index));
            }

            return View(khoaHoc);
        }

        // POST: KhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khoaHoc = await _context.KhoaHoc.FindAsync(id);
            if (khoaHoc != null)
            {
                // Kiểm tra xem khóa học đã có học viên đăng ký chưa
                var dangKy = await _context.DangKyKhoaHoc
                    .AnyAsync(dk => dk.MaKhoaHoc == id && !dk.DaHuy);
                
                if (dangKy)
                {
                    TempData["ErrorMessage"] = "Không thể xóa khóa học đã có học viên đăng ký!";
                    return RedirectToAction(nameof(Index));
                }
                
                _context.KhoaHoc.Remove(khoaHoc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa khóa học thành công!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool KhoaHocExists(string id)
        {
            return _context.KhoaHoc.Any(e => e.MaKhoaHoc == id);
        }
        
        // GET: KhoaHoc/DanhSachKhoaHoc - Hiển thị cho khách không đăng nhập
        public async Task<IActionResult> DanhSachKhoaHoc()
        {
            var khoaHocList = await _context.KhoaHoc
                .Select(k => new KhoaHocViewModel
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    GiangVien = k.GiangVien,
                    ThoiGianKhaiGiang = k.ThoiGianKhaiGiang,
                    HocPhi = k.HocPhi,
                    SoLuongHocVienToiDa = k.SoLuongHocVienToiDa,
                    SoLuongDangKy = _context.DangKyKhoaHoc.Count(dk => dk.MaKhoaHoc == k.MaKhoaHoc && !dk.DaHuy)
                })
                .ToListAsync();

            return View(khoaHocList);
        }
    }

    // ViewModel cho hiển thị thông tin khóa học và số lượng đã đăng ký
    public class KhoaHocViewModel
    {
        public string MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string GiangVien { get; set; }
        public DateTime ThoiGianKhaiGiang { get; set; }
        public decimal HocPhi { get; set; }
        public int SoLuongHocVienToiDa { get; set; }
        public int SoLuongDangKy { get; set; }
    }
}