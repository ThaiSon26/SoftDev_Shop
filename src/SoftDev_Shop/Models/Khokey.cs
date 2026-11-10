using System;
using System.Collections.Generic;

namespace SoftDev_Shop.Models;

public partial class Khokey
{
    public uint Id { get; set; }

    public uint GoiBanQuyenId { get; set; }

    public uint? DoiTacId { get; set; }

    public string MaKey { get; set; } = null!;

    public string LoaiKey { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public uint? ChiTietDonHangId { get; set; }

    public string? GhiChu { get; set; }

    public DateTime? ThoiDiemGiaoKey { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual Chitietdonhang? ChiTietDonHang { get; set; }

    public virtual Doitac? DoiTac { get; set; }

    public virtual Goibanquyen GoiBanQuyen { get; set; } = null!;
}
