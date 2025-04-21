using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrungTamDaoTao.Models
{
public class DangKyKhoaHoc
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã học viên là bắt buộc.")]
    [ForeignKey("HocVien")] // Chỉ định khóa ngoài
    public string MaHocVien { get; set; }

    [Required(ErrorMessage = "Mã khóa học là bắt buộc.")]
    [ForeignKey("KhoaHoc")] // Chỉ định khóa ngoài
    public string MaKhoaHoc { get; set; }

    [Required(ErrorMessage = "Ngày đăng ký là bắt buộc.")]
    [DataType(DataType.Date)]
    [Range(typeof(DateTime), "01/01/2000", "01/01/2100", ErrorMessage = "Ngày đăng ký không hợp lệ.")]
    public DateTime NgayDangKy { get; set; }

    public bool DaHuy { get; set; } = false;

    [DataType(DataType.Date)]
    public DateTime? NgayHuy { get; set; }

    public virtual HocVien HocVien { get; set; }
    public virtual KhoaHoc KhoaHoc { get; set; }
}
}
