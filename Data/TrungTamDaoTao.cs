using Microsoft.EntityFrameworkCore;
using TrungTamDaoTao.Models;

namespace TrungTamDaoTao.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<HocVien> HocVien { get; set; }
        public DbSet<KhoaHoc> KhoaHoc { get; set; }
        public DbSet<DangKyKhoaHoc> DangKyKhoaHoc { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mô hình dữ liệu
            modelBuilder.Entity<DangKyKhoaHoc>()
                .HasOne(d => d.HocVien)
                .WithMany(h => h.DangKyKhoaHoc)
                .HasForeignKey(d => d.MaHocVien);

            modelBuilder.Entity<DangKyKhoaHoc>()
                .HasOne(d => d.KhoaHoc)
                .WithMany(k => k.DangKyKhoaHoc)
                .HasForeignKey(d => d.MaKhoaHoc);

            // Seed dữ liệu mẫu (nếu cần)
            // Admin account
            modelBuilder.Entity<HocVien>().HasData(
                new HocVien
                {
                    MaHocVien = "ADMIN001",
                    HoTen = "Admin",
                    NgaySinh = new DateTime(1990, 1, 1),
                    SoDienThoai = "0123456789",
                    Email = "admin@trungtamdaotao.com",
                    TaiKhoan = "admin",
                    // Mật khẩu: admin123 (đã hash)
                    PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                    VaiTro = "QuanTriVien"
                }
            );
        }
    }
}