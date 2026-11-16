using SoftDev_Shop.Models;

namespace SoftDev_Shop.ViewModels;

public class AdminDashboardViewModel
{
    public decimal TongDoanhThu { get; set; }
    public int TongDonHang { get; set; }
    public int TongNguoiDung { get; set; }
    public int TongSanPham { get; set; }

    public List<Donhang> DonHangMoiNhat { get; set; } = new();
    public List<MonthlyRevenueItem> DoanhThuTheoThang { get; set; } = new();
}

public class MonthlyRevenueItem
{
    public string Thang { get; set; } = string.Empty;
    public decimal DoanhThu { get; set; }
}
