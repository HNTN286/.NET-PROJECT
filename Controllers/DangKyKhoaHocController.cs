using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrungTamDaoTao.Data;
using TrungTamDaoTao.Models;
using TrungTamDaoTao.ViewModels;

namespace TrungTamDaoTao.Controllers
{
    [Authorize] // Yêu cầu đăng nhập để sử dụng controller này
    public class DangKyKhoaHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DangKyKhoaHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DangKyKhoaHoc - Hiển thị danh sách khóa học có thể đăng ký
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var khoaHocList = await _context.KhoaHoc
                .Select(kh => new DangKyKhoaHocViewModel
                {
                    MaKhoaHoc = kh.MaKhoaHoc,
                    TenKhoaHoc = kh.TenKhoaHoc,
                    GiangVien = kh.GiangVien,
                    ThoiGianKhaiGiang = kh.ThoiGianKhaiGiang,
                    HocPhi = kh.HocPhi,
                    SoLuongHocVienToiDa = kh.SoLuongHocVienToiDa,
                    SoLuongHocVienDaDangKy = kh.DangKyKhoaHoc.Count(dk => !dk.DaHuy),
                    ConChoTrong = kh.SoLuongHocVienToiDa > kh.DangKyKhoaHoc.Count(dk => !dk.DaHuy)
                })
                .ToListAsync();

            return View(khoaHocList);
        }

        // GET: DangKyKhoaHoc/DangKy/5 - Hiển thị form xác nhận đăng ký
        public async Task<IActionResult> DangKy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHoc.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu khóa học đã đầy
            int soLuongHocVienHienTai = await _context.DangKyKhoaHoc
                .Where(dk => dk.MaKhoaHoc == id && !dk.DaHuy)
                .CountAsync();

            if (soLuongHocVienHienTai >= khoaHoc.SoLuongHocVienToiDa)
            {
                TempData["ErrorMessage"] = "Khóa học đã đầy, không thể đăng ký!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xem học viên đã đăng ký khóa học này chưa
            var maHocVien = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var daDangKy = await _context.DangKyKhoaHoc
                .AnyAsync(dk => dk.MaHocVien == maHocVien && dk.MaKhoaHoc == id && !dk.DaHuy);
            
            if (daDangKy)
            {
                TempData["ErrorMessage"] = "Bạn đã đăng ký khóa học này rồi!";
                return RedirectToAction(nameof(Index));
            }

            // Truyền thông tin khóa học để xác nhận
            return View(khoaHoc);
        }

        // POST: DangKyKhoaHoc/DangKy/5 - Xử lý đăng ký khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(string id, [Bind("MaKhoaHoc")] KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc)
            {
                return NotFound();
            }

            var khoaHocFromDb = await _context.KhoaHoc.FindAsync(id);
            if (khoaHocFromDb == null)
            {
                return NotFound();
            }

            // Lấy mã học viên từ người dùng đã đăng nhập
            var maHocVien = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Kiểm tra nếu khóa học đã đầy
            int soLuongHocVienHienTai = await _context.DangKyKhoaHoc
                .Where(dk => dk.MaKhoaHoc == id && !dk.DaHuy)
                .CountAsync();

            if (soLuongHocVienHienTai >= khoaHocFromDb.SoLuongHocVienToiDa)
            {
                TempData["ErrorMessage"] = "Khóa học đã đầy, không thể đăng ký!";
                return RedirectToAction(nameof(Index));
            }
            
            // Kiểm tra xem học viên đã đăng ký khóa học này chưa
            var daDangKy = await _context.DangKyKhoaHoc
                .AnyAsync(dk => dk.MaHocVien == maHocVien && dk.MaKhoaHoc == id && !dk.DaHuy);
            
            if (daDangKy)
            {
                TempData["ErrorMessage"] = "Bạn đã đăng ký khóa học này rồi!";
                return RedirectToAction(nameof(Index));
            }

            // Tạo đối tượng đăng ký khóa học mới
            var dangKyKhoaHoc = new DangKyKhoaHoc
            {
                MaHocVien = maHocVien,
                MaKhoaHoc = id,
                NgayDangKy = DateTime.Now,
                DaHuy = false
            };

            _context.Add(dangKyKhoaHoc);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Đăng ký khóa học thành công!";
            return RedirectToAction(nameof(KhoaHocCuaToi));
        }

        // GET: DangKyKhoaHoc/KhoaHocCuaToi - Hiển thị danh sách khóa học đã đăng ký
        public async Task<IActionResult> KhoaHocCuaToi()
        {
            var maHocVien = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var dangKyList = await _context.DangKyKhoaHoc
                .Include(dk => dk.KhoaHoc)
                .Where(dk => dk.MaHocVien == maHocVien)
                .OrderByDescending(dk => dk.NgayDangKy)
                .ToListAsync();

            return View(dangKyList);
        }

        // GET: DangKyKhoaHoc/Huy/5 - Hiển thị form xác nhận hủy đăng ký
        public async Task<IActionResult> Huy(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maHocVien = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var dangKyKhoaHoc = await _context.DangKyKhoaHoc
                .Include(dk => dk.KhoaHoc)
                .FirstOrDefaultAsync(dk => dk.Id == id && dk.MaHocVien == maHocVien);
                
            if (dangKyKhoaHoc == null)
            {
                return NotFound();
            }

            // Kiểm tra xem có thể hủy không (trước ngày khai giảng)
            if (DateTime.Now >= dangKyKhoaHoc.KhoaHoc.ThoiGianKhaiGiang)
            {
                TempData["ErrorMessage"] = "Không thể hủy đăng ký sau ngày khai giảng!";
                return RedirectToAction(nameof(KhoaHocCuaToi));
            }

            if (dangKyKhoaHoc.DaHuy)
            {
                TempData["ErrorMessage"] = "Đăng ký này đã được hủy trước đó!";
                return RedirectToAction(nameof(KhoaHocCuaToi));
            }

            return View(dangKyKhoaHoc);
        }

        // POST: DangKyKhoaHoc/Huy/5 - Xử lý hủy đăng ký
        [HttpPost, ActionName("Huy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyConfirmed(int id)
        {
            var maHocVien = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var dangKyKhoaHoc = await _context.DangKyKhoaHoc
                .Include(dk => dk.KhoaHoc)
                .FirstOrDefaultAsync(dk => dk.Id == id && dk.MaHocVien == maHocVien);
                
            if (dangKyKhoaHoc == null)
            {
                return NotFound();
            }

            // Kiểm tra xem có thể hủy không (trước ngày khai giảng)
            if (DateTime.Now >= dangKyKhoaHoc.KhoaHoc.ThoiGianKhaiGiang)
            {
                TempData["ErrorMessage"] = "Không thể hủy đăng ký sau ngày khai giảng!";
                return RedirectToAction(nameof(KhoaHocCuaToi));
            }

            if (dangKyKhoaHoc.DaHuy)
            {
                TempData["ErrorMessage"] = "Đăng ký này đã được hủy trước đó!";
                return RedirectToAction(nameof(KhoaHocCuaToi));
            }

            // Thực hiện hủy đăng ký
            dangKyKhoaHoc.DaHuy = true;
            dangKyKhoaHoc.NgayHuy = DateTime.Now;
            
            _context.Update(dangKyKhoaHoc);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Hủy đăng ký khóa học thành công!";
            return RedirectToAction(nameof(KhoaHocCuaToi));
        }
        
        // GET: DangKyKhoaHoc/ThongKe - Thống kê số lượng học viên theo khóa học (cho quản trị viên)
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> ThongKe()
        {
            var thongKeList = await _context.KhoaHoc
                .Select(kh => new ThongKeKhoaHocViewModel
                {
                    MaKhoaHoc = kh.MaKhoaHoc,
                    TenKhoaHoc = kh.TenKhoaHoc,
                    ThoiGianKhaiGiang = kh.ThoiGianKhaiGiang,
                    SoLuongHocVienToiDa = kh.SoLuongHocVienToiDa,
                    SoLuongHocVienDaDangKy = kh.DangKyKhoaHoc.Count(dk => !dk.DaHuy),
                    SoLuongHocVienDaHuy = kh.DangKyKhoaHoc.Count(dk => dk.DaHuy),
                    TiLeDangKy = (float)kh.DangKyKhoaHoc.Count(dk => !dk.DaHuy) / kh.SoLuongHocVienToiDa * 100,
                    DoanhThu = kh.HocPhi * kh.DangKyKhoaHoc.Count(dk => !dk.DaHuy)
                })
                .ToListAsync();

            return View(thongKeList);
        }
    }
}