using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrungTamDaoTao.Models
{
    public class KhoaHoc
    {
        [Key]
        [Required(ErrorMessage = "Mã khóa học là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Mã khóa học không được vượt quá 20 ký tự.")]
        public string MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Tên khóa học là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên khóa học không được vượt quá 100 ký tự.")]
        public string TenKhoaHoc { get; set; }

        [Required(ErrorMessage = "Giảng viên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên giảng viên không được vượt quá 100 ký tự.")]
        public string GiangVien { get; set; }

        [Required(ErrorMessage = "Thời gian khai giảng là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime ThoiGianKhaiGiang { get; set; }

        [Required(ErrorMessage = "Học phí là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Học phí phải là số dương.")]
        public decimal HocPhi { get; set; }

        [Required(ErrorMessage = "Số lượng học viên tối đa là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng học viên tối đa phải lớn hơn 0.")]
        public int SoLuongHocVienToiDa { get; set; }

        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHoc { get; set; } = new List<DangKyKhoaHoc>();
    }
}