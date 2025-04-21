using System;

namespace TrungTamDaoTao.ViewModels
{
    public class DangKyKhoaHocViewModel
    {
        public string MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string GiangVien { get; set; }
        public DateTime ThoiGianKhaiGiang { get; set; }
        public decimal HocPhi { get; set; }
        public int SoLuongHocVienToiDa { get; set; }
        public int SoLuongHocVienDaDangKy { get; set; }
        public bool ConChoTrong { get; set; }
    }

    public class ThongKeKhoaHocViewModel
    {
        public string MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public DateTime ThoiGianKhaiGiang { get; set; }
        public int SoLuongHocVienToiDa { get; set; }
        public int SoLuongHocVienDaDangKy { get; set; }
        public int SoLuongHocVienDaHuy { get; set; }
        public float TiLeDangKy { get; set; }
        public decimal DoanhThu { get; set; }
    }
}