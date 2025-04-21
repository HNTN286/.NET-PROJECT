// ViewModels/DashboardViewModel.cs
using System.Collections.Generic;
using TrungTamDaoTao.Models;

namespace TrungTamDaoTao.ViewModels
{
    public class DashboardViewModel
    {
        public int TongSoKhoaHoc { get; set; }
        public int TongSoHocVien { get; set; }
        public int TongSoDangKy { get; set; }
        public decimal TongDoanhThu { get; set; }
        public List<KhoaHoc> KhoaHocSapKhaiGiang { get; set; }
        public List<DangKyKhoaHoc> DangKyGanDay { get; set; }
    }
}