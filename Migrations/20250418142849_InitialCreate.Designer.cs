﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrungTamDaoTao.Data;

#nullable disable

namespace Quanlytrungtamdaotao.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250418142849_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TrungTamDaoTao.Models.DangKyKhoaHoc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("DaHuy")
                        .HasColumnType("bit");

                    b.Property<string>("MaHocVien")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("MaKhoaHoc")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("NgayDangKy")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayHuy")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MaHocVien");

                    b.HasIndex("MaKhoaHoc");

                    b.ToTable("DangKyKhoaHoc");
                });

            modelBuilder.Entity("TrungTamDaoTao.Models.HocVien", b =>
                {
                    b.Property<string>("MaHocVien")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("NgaySinh")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("SoDienThoai")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TaiKhoan")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("VaiTro")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MaHocVien");

                    b.ToTable("HocVien");
                });

            modelBuilder.Entity("TrungTamDaoTao.Models.KhoaHoc", b =>
                {
                    b.Property<string>("MaKhoaHoc")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("GiangVien")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("HocPhi")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SoLuongHocVienToiDa")
                        .HasColumnType("int");

                    b.Property<string>("TenKhoaHoc")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ThoiGianKhaiGiang")
                        .HasColumnType("datetime2");

                    b.HasKey("MaKhoaHoc");

                    b.ToTable("KhoaHoc");
                });

            modelBuilder.Entity("TrungTamDaoTao.Models.DangKyKhoaHoc", b =>
                {
                    b.HasOne("TrungTamDaoTao.Models.HocVien", "HocVien")
                        .WithMany("DangKyKhoaHoc")
                        .HasForeignKey("MaHocVien")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrungTamDaoTao.Models.KhoaHoc", "KhoaHoc")
                        .WithMany("DangKyKhoaHoc")
                        .HasForeignKey("MaKhoaHoc")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HocVien");

                    b.Navigation("KhoaHoc");
                });

            modelBuilder.Entity("TrungTamDaoTao.Models.HocVien", b =>
                {
                    b.Navigation("DangKyKhoaHoc");
                });

            modelBuilder.Entity("TrungTamDaoTao.Models.KhoaHoc", b =>
                {
                    b.Navigation("DangKyKhoaHoc");
                });
#pragma warning restore 612, 618
        }
    }
}
