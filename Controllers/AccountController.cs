using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TrungTamDaoTao.Data;
using TrungTamDaoTao.Models;
using TrungTamDaoTao.ViewModels;

namespace TrungTamDaoTao.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _context.HocVien
                    .FirstOrDefaultAsync(m => m.TaiKhoan == model.TaiKhoan);

                if ((user != null) && (model.Password == user.PasswordHash))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.HoTen),
                        new Claim(ClaimTypes.NameIdentifier, user.MaHocVien),
                        new Claim(ClaimTypes.Role, user.VaiTro)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng.");
                return View(model);
            }

            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tài khoản đã tồn tại chưa
                if (await _context.HocVien.AnyAsync(x => x.TaiKhoan == model.TaiKhoan))
                {
                    ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại chưa
                if (await _context.HocVien.AnyAsync(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                // Tạo mã học viên mới
                string maHocVien = GenerateMaHocVien();

                var hocVien = new HocVien
                {
                    MaHocVien = maHocVien,
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    SoDienThoai = model.SoDienThoai,
                    Email = model.Email,
                    TaiKhoan = model.TaiKhoan,
                    PasswordHash = model.Password,
                    VaiTro = "HocVien"
                };

                _context.Add(hocVien);
                await _context.SaveChangesAsync();

                // Tự động đăng nhập sau khi đăng ký
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, hocVien.HoTen),
                    new Claim(ClaimTypes.NameIdentifier, hocVien.MaHocVien),
                    new Claim(ClaimTypes.Role, hocVien.VaiTro)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                // return RedirectToAction("Index", "Home");
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Phương thức tạo mã học viên mới
        private string GenerateMaHocVien()
        {
            // Lấy mã học viên cuối cùng để tạo mã mới
            var lastHocVien = _context.HocVien
                .OrderByDescending(h => h.MaHocVien)
                .FirstOrDefault();

            string prefix = "HV";
            int number = 1;

            // Nếu đã có học viên, tăng số lên 1
            if (lastHocVien != null && lastHocVien.MaHocVien.StartsWith(prefix))
            {
                string numberStr = lastHocVien.MaHocVien.Substring(prefix.Length);
                if (int.TryParse(numberStr, out int lastNumber))
                {
                    number = lastNumber + 1;
                }
            }

            return $"{prefix}{number:D5}";
        }

        // Mã hóa mật khẩu
        // private string HashPassword(string password)
        // {
        //     using (var sha256 = SHA256.Create())
        //     {
        //         var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //         return Convert.ToBase64String(hashedBytes);
        //     }
        // }

        // Xác thực mật khẩu
        // private bool VerifyPassword(string password, string hashedPassword)
        // {
        //     string hashed = HashPassword(password);
        //     return hashed == hashedPassword;
        // }
    }
}