// ViewModels/DoanhThuTheoThoiGianViewModel.cs
using System.Collections.Generic;

namespace TrungTamDaoTao.ViewModels
{
    public class DoanhThuTheoThoiGianViewModel
    {
        public int Nam { get; set; }
        public int? Thang { get; set; }
        public Dictionary<int, decimal> DoanhThuTheoThang { get; set; }
        public Dictionary<int, decimal> DoanhThuTheoNgay { get; set; }
        public decimal TongDoanhThu { get; set; }
    }
}