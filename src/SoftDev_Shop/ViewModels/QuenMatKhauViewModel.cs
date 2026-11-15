using System.ComponentModel.DataAnnotations;

namespace SoftDev_Shop.ViewModels;

public class QuenMatKhauViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
    [StringLength(190, ErrorMessage = "Email không được vượt quá 190 ký tự.")]
    public string Email { get; set; } = string.Empty;
}

