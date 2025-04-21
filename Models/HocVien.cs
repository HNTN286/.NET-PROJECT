using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrungTamDaoTao.Models
{
    public class HocVien
    {
        [Key]
        [Required(ErrorMessage = "Mã học viên là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Mã học viên không được vượt quá 20 ký tự.")]
        public string MaHocVien { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tài khoản không được vượt quá 50 ký tự.")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(256, ErrorMessage = "Mật khẩu không được vượt quá 256 ký tự.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc.")]
        public string VaiTro { get; set; } = "HocVien";

        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHoc { get; set; } = new List<DangKyKhoaHoc>();
    }
}