using System;
using System.Collections.Generic;
using TrungTamDaoTao.Models;

namespace TrungTamDaoTao.ViewModels
{
    // ViewModel chi tiết một đăng ký khóa học
    public class ChiTietDangKyViewModel
    {
        public int Id { get; set; }
        public string MaHocVien { get; set; }
        public string TenHocVien { get; set; }
        public string EmailHocVien { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime? NgayHuy { get; set; }
        public bool DaHuy { get; set; }
        public DateTime ThoiGianKhaiGiang { get; set; }
    }

    // ViewModel danh sách đăng ký cho một khóa học cụ thể
    public class ChiTietKhoaHocDangKyViewModel
    {
        public KhoaHoc KhoaHoc { get; set; }
        public List<ChiTietDangKyViewModel> DanhSachDangKy { get; set; }
        public int TongSoDangKy { get; set; }
        public int SoDangKyHieuLuc { get; set; }
        public int SoDangKyDaHuy { get; set; }
    }

    // ViewModel cho danh sách tất cả các đăng ký
    public class DanhSachDangKyViewModel
    {
        public int Id { get; set; }
        public string MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string MaHocVien { get; set; }
        public string TenHocVien { get; set; }
        public string EmailHocVien { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime? NgayHuy { get; set; }
        public bool DaHuy { get; set; }
        public DateTime ThoiGianKhaiGiang { get; set; }
    }
}