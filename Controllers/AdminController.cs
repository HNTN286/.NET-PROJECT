using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrungTamDaoTao.Data;
using TrungTamDaoTao.Models;
using TrungTamDaoTao.ViewModels;

namespace TrungTamDaoTao.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard - Tổng quan hệ thống
        public async Task<IActionResult> Dashboard()
        {
            // Lấy thông tin tổng quan cho Dashboard
            var dashboardViewModel = new DashboardViewModel
            {
                TongSoKhoaHoc = await _context.KhoaHoc.CountAsync(),
                TongSoHocVien = await _context.HocVien.CountAsync(),
                TongSoDangKy = await _context.DangKyKhoaHoc.CountAsync(),
                TongDoanhThu = await _context.DangKyKhoaHoc
                    .Where(dk => !dk.DaHuy)
                    .Join(_context.KhoaHoc,
                        dangky => dangky.MaKhoaHoc,
                        khoahoc => khoahoc.MaKhoaHoc,
                        (dangky, khoahoc) => khoahoc.HocPhi)
                    .SumAsync(),
                KhoaHocSapKhaiGiang = await _context.KhoaHoc
                    .Where(kh => kh.ThoiGianKhaiGiang > DateTime.Now)
                    .OrderBy(kh => kh.ThoiGianKhaiGiang)
                    .Take(5)
                    .ToListAsync(),
                DangKyGanDay = await _context.DangKyKhoaHoc
                    .Include(dk => dk.HocVien)
                    .Include(dk => dk.KhoaHoc)
                    .OrderByDescending(dk => dk.NgayDangKy)
                    .Take(10)
                    .ToListAsync()
            };

            return View(dashboardViewModel);
        }

        // Thống kê doanh thu theo khóa học
        public async Task<IActionResult> DoanhThuTheoKhoaHoc(DateTime? tuNgay, DateTime? denNgay)
        {
            // Mặc định là tháng hiện tại nếu không có tham số
            if (!tuNgay.HasValue)
                tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!denNgay.HasValue)
                denNgay = DateTime.Now;

            var thongKe = await _context.KhoaHoc
                .Select(kh => new DoanhThuKhoaHocViewModel
                {
                    MaKhoaHoc = kh.MaKhoaHoc,
                    TenKhoaHoc = kh.TenKhoaHoc,
                    SoLuongDangKy = kh.DangKyKhoaHoc.Count(dk => !dk.DaHuy && dk.NgayDangKy >= tuNgay && dk.NgayDangKy <= denNgay),
                    TongDoanhThu = kh.HocPhi * kh.DangKyKhoaHoc.Count(dk => !dk.DaHuy && dk.NgayDangKy >= tuNgay && dk.NgayDangKy <= denNgay)
                })
                .Where(x => x.SoLuongDangKy > 0)
                .OrderByDescending(x => x.TongDoanhThu)
                .ToListAsync();

            // Tính tổng doanh thu
            decimal tongDoanhThu = thongKe.Sum(t => t.TongDoanhThu);

            var viewModel = new DoanhThuTheoKhoaHocViewModel
            {
                TuNgay = tuNgay.Value,
                DenNgay = denNgay.Value,
                DoanhThuKhoaHoc = thongKe,
                TongDoanhThu = tongDoanhThu
            };

            return View(viewModel);
        }

        // Thống kê doanh thu theo thời gian
        public async Task<IActionResult> DoanhThuTheoThoiGian(int? nam, int? thang)
        {
            // Mặc định là năm và tháng hiện tại
            if (!nam.HasValue)
                nam = DateTime.Now.Year;
            
            var doanhThuViewModel = new DoanhThuTheoThoiGianViewModel
            {
                Nam = nam.Value
            };

            if (thang.HasValue)
            {
                // Nếu có tháng, lấy doanh thu theo ngày trong tháng
                doanhThuViewModel.Thang = thang.Value;
                DateTime ngayDauThang = new DateTime(nam.Value, thang.Value, 1);
                DateTime ngayCuoiThang = ngayDauThang.AddMonths(1).AddDays(-1);

                // Lấy dữ liệu doanh thu theo từng ngày trong tháng
                var doanhThuTheoNgay = await _context.DangKyKhoaHoc
                    .Where(dk => !dk.DaHuy && dk.NgayDangKy >= ngayDauThang && dk.NgayDangKy <= ngayCuoiThang)
                    .GroupBy(dk => dk.NgayDangKy.Day)
                    .Select(g => new
                    {
                        Ngay = g.Key,
                        DoanhThu = g.Sum(dk => dk.KhoaHoc.HocPhi)
                    })
                    .OrderBy(x => x.Ngay)
                    .ToListAsync();

                doanhThuViewModel.DoanhThuTheoNgay = new Dictionary<int, decimal>();
                
                // Đảm bảo có đủ dữ liệu cho tất cả các ngày trong tháng
                for (int i = 1; i <= DateTime.DaysInMonth(nam.Value, thang.Value); i++)
                {
                    var data = doanhThuTheoNgay.FirstOrDefault(d => d.Ngay == i);
                    doanhThuViewModel.DoanhThuTheoNgay.Add(i, data != null ? data.DoanhThu : 0);
                }

                doanhThuViewModel.TongDoanhThu = doanhThuTheoNgay.Sum(d => d.DoanhThu);
            }
            else
            {
                // Nếu không có tháng, lấy doanh thu theo tháng trong năm
                DateTime ngayDauNam = new DateTime(nam.Value, 1, 1);
                DateTime ngayCuoiNam = new DateTime(nam.Value, 12, 31);

                // Lấy dữ liệu doanh thu theo từng tháng trong năm
                var doanhThuTheoThang = await _context.DangKyKhoaHoc
                    .Where(dk => !dk.DaHuy && dk.NgayDangKy >= ngayDauNam && dk.NgayDangKy <= ngayCuoiNam)
                    .GroupBy(dk => dk.NgayDangKy.Month)
                    .Select(g => new
                    {
                        Thang = g.Key,
                        DoanhThu = g.Sum(dk => dk.KhoaHoc.HocPhi)
                    })
                    .OrderBy(x => x.Thang)
                    .ToListAsync();

                doanhThuViewModel.DoanhThuTheoThang = new Dictionary<int, decimal>();
                
                // Đảm bảo có đủ dữ liệu cho tất cả các tháng trong năm
                for (int i = 1; i <= 12; i++)
                {
                    var data = doanhThuTheoThang.FirstOrDefault(d => d.Thang == i);
                    doanhThuViewModel.DoanhThuTheoThang.Add(i, data != null ? data.DoanhThu : 0);
                }

                doanhThuViewModel.TongDoanhThu = doanhThuTheoThang.Sum(d => d.DoanhThu);
            }

            return View(doanhThuViewModel);
        }

        // // Quản lý khóa học dành cho Admin
        // public async Task<IActionResult> QuanLyKhoaHoc()
        // {
        //     var khoaHocList = await _context.KhoaHoc
        //         .OrderByDescending(k => k.ThoiGianKhaiGiang)
        //         .ToListAsync();
        //     return View(khoaHocList);
        // }

        // // Quản lý đăng ký khóa học dành cho Admin
        // public async Task<IActionResult> QuanLyDangKyKhoaHoc()
        // {
        //     var dangKyList = await _context.DangKyKhoaHoc
        //         .Include(dk => dk.HocVien)
        //         .Include(dk => dk.KhoaHoc)
        //         .OrderByDescending(dk => dk.NgayDangKy)
        //         .ToListAsync();
        //     return View(dangKyList);
        // }
    }
}