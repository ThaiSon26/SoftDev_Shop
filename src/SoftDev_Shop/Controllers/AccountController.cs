using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftDev_Shop.Data;
using SoftDev_Shop.Models;
using SoftDev_Shop.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SoftDev_Shop.Controllers;

public class AccountController : Controller
{
    private readonly QLBanPhanMemContext _context;

    public AccountController(QLBanPhanMemContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult DangNhap(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangNhap(DangNhapViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLower();
        var user = await _context.Taikhoans
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
            return View(model);
        }

        if (user.ConHoatDong == false)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động.");
            return View(model);
        }

        bool isPasswordValid = false;
        try
        {
            isPasswordValid = BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhauBam);
        }
        catch
        {
            isPasswordValid = (model.MatKhau == user.MatKhauBam);
        }

        if (!isPasswordValid)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.HoTen),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.VaiTro ?? "Khách hàng")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.GhiNhoDangNhap,
            ExpiresUtc = model.GhiNhoDangNhap ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(2)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        // Phân quyền điều hướng sau khi đăng nhập:
        // Nếu là Quản trị hoặc Nhân viên -> Vào Dashboard Quản trị
        // Nếu là Khách hàng -> Vào Trang chủ Cửa hàng
        if (string.Equals(user.VaiTro, "Quản trị", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.VaiTro, "Nhân viên", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult DangKy()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangKy(DangKyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLower();
        bool emailExists = await _context.Taikhoans.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Email này đã được sử dụng. Vui lòng chọn email khác.");
            return View(model);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);

        var newUser = new Taikhoan
        {
            HoTen = model.HoTen.Trim(),
            Email = normalizedEmail,
            SoDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoai) ? null : model.SoDienThoai.Trim(),
            MatKhauBam = passwordHash,
            VaiTro = "Khách hàng",
            ConHoatDong = true,
            NgayTao = DateTime.UtcNow,
            NgayCapNhat = DateTime.UtcNow
        };

        _context.Taikhoans.Add(newUser);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
        return RedirectToAction("DangNhap");
    }

    [HttpGet]
    public IActionResult QuenMatKhau()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuenMatKhau(QuenMatKhauViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLower();
        var user = await _context.Taikhoans.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (user != null && user.ConHoatDong != false)
        {
            // Tạo token đặt lại mật khẩu an toàn dựa trên thông tin người dùng và thời gian (hết hạn trong 30 phút)
            var expiry = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds();
            var rawTokenData = $"{user.Id}:{user.Email}:{user.MatKhauBam}:{expiry}";
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawTokenData));
            var signature = Convert.ToHexString(hashBytes).ToLower();
            var token = $"{expiry}_{signature}";

            var resetLink = Url.Action("DatLaiMatKhau", "Account", new { email = user.Email, token }, Request.Scheme);
            ViewBag.ResetLink = resetLink;
        }

        ViewBag.SubmittedEmail = model.Email;
        return View("QuenMatKhauXacNhan");
    }

    [HttpGet]
    public async Task<IActionResult> DatLaiMatKhau(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            TempData["ErrorMessage"] = "Liên kết đặt lại mật khẩu không hợp lệ.";
            return RedirectToAction("DangNhap");
        }

        var normalizedEmail = email.Trim().ToLower();
        var user = await _context.Taikhoans.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        if (user == null || !ValidateResetToken(user, token))
        {
            TempData["ErrorMessage"] = "Liên kết đặt lại mật khẩu đã hết hạn hoặc không hợp lệ.";
            return RedirectToAction("DangNhap");
        }

        var model = new DatLaiMatKhauViewModel
        {
            Email = user.Email,
            Token = token
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLower();
        var user = await _context.Taikhoans.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        if (user == null || !ValidateResetToken(user, model.Token))
        {
            ModelState.AddModelError(string.Empty, "Yêu cầu đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
            return View(model);
        }

        user.MatKhauBam = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
        user.NgayCapNhat = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập bằng mật khẩu mới.";
        return RedirectToAction("DangNhap");
    }

    private static bool ValidateResetToken(Taikhoan user, string token)
    {
        var parts = token.Split('_');
        if (parts.Length != 2) return false;

        if (!long.TryParse(parts[0], out var expiry)) return false;
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiry) return false;

        var rawTokenData = $"{user.Id}:{user.Email}:{user.MatKhauBam}:{expiry}";
        using var sha = SHA256.Create();
        var expectedHash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(rawTokenData))).ToLower();

        return string.Equals(parts[1], expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangXuat()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("DangNhap");
    }

    [HttpGet]
    public IActionResult TuChoiTruyCap()
    {
        return View();
    }
}
