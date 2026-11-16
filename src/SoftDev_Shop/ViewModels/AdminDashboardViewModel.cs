using SoftDev_Shop.Models;

namespace SoftDev_Shop.ViewModels;

public class AdminDashboardViewModel
{
    public decimal TongDoanhThu { get; set; }
    public int TongDonHang { get; set; }
    public int TongNguoiDung { get; set; }
    public double DanhGiaTrungBinh { get; set; } = 4.8;
    public int TongSanPham { get; set; }

    public List<RecentOrderItem> DonHangGanDay { get; set; } = new();
    public List<TopProductItem> SanPhamBanChay { get; set; } = new();
    public List<CategoryStatItem> ThongKeDanhMuc { get; set; } = new();
    public SystemStatusItem ThongTinHeThong { get; set; } = new();
}

public class RecentOrderItem
{
    public string MaDonHang { get; set; } = string.Empty;
    public string TenKhachHang { get; set; } = string.Empty;
    public string TenSanPham { get; set; } = string.Empty;
    public decimal SoTien { get; set; }
    public string TrangThai { get; set; } = string.Empty; // "Đã thanh toán", "Đang xử lý", "Chờ xử lý"
    public string ThoiGian { get; set; } = string.Empty; // "2 phút trước", "5 phút trước"
    public string Avatar { get; set; } = string.Empty;
}

public class TopProductItem
{
    public int Hang { get; set; }
    public string TenSanPham { get; set; } = string.Empty;
    public string MoTaNgan { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public string PhanTramTang { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;
}

public class CategoryStatItem
{
    public string TenDanhMuc { get; set; } = string.Empty;
    public int PhanTram { get; set; }
    public string MauSac { get; set; } = string.Empty;
}

public class SystemStatusItem
{
    public string MayChu { get; set; } = "localhost";
    public string CoSoDuLieu { get; set; } = "MySQL 8.0";
    public string DungLuong { get; set; } = "2.1 GB / 10 GB";
    public int PhanTramDungLuong { get; set; } = 21;
    public string ThoiGianHoatDong { get; set; } = "12 ngày, 4 giờ";
    public string TrangThai { get; set; } = "Hoạt động tốt";
}
