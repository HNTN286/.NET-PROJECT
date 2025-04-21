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
        }
    }
}