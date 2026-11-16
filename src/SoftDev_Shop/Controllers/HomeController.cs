using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftDev_Shop.Data;
using SoftDev_Shop.Models;

namespace SoftDev_Shop.Controllers;

public class HomeController : Controller
{
    private readonly QLBanPhanMemContext _context;

    public HomeController(QLBanPhanMemContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            ViewBag.Categories = await _context.Danhmucs
                .Where(d => d.ConHoatDong == true)
                .OrderBy(d => d.ThuTu)
                .Take(8)
                .ToListAsync();

            ViewBag.Partners = await _context.Doitacs
                .Where(d => d.ConHoatDong == true)
                .Take(6)
                .ToListAsync();

            ViewBag.FeaturedProducts = await _context.Sanphams
                .Include(s => s.DanhMuc)
                .Include(s => s.DoiTac)
                .Where(s => s.ConHoatDong == true && s.LaNoiBat == true)
                .OrderByDescending(s => s.LuotXem)
                .Take(8)
                .ToListAsync();
        }
        catch
        {
            // Fallback nếu chưa kết nối tới database
            ViewBag.Categories = new List<Danhmuc>();
            ViewBag.Partners = new List<Doitac>();
            ViewBag.FeaturedProducts = new List<Sanpham>();
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
