// ViewModels/DoanhThuTheoKhoaHocViewModel.cs
using System;
using System.Collections.Generic;

namespace TrungTamDaoTao.ViewModels
{
    public class DoanhThuTheoKhoaHocViewModel
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public List<DoanhThuKhoaHocViewModel> DoanhThuKhoaHoc { get; set; }
        public decimal TongDoanhThu { get; set; }
    }
}